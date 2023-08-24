using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using BrawlLib.Internal.IO;
using BrawlLib.Modeling.Collada;
using BrawlLib.SSBB.ResourceNodes;
using BrawlLib.SSBB.Types;
using BrawlLib.Wii.Compression;
using BrawlLib.Wii.Textures;
using Sledge.Formats.Map.Formats;
using Sledge.Formats.Map.Objects;

namespace StageHammer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Args should have at least 3 parameters
            if (args.Length < 3)
            {
                Console.WriteLine("Not enough arguments.");
                return;
            }

            // Check to see if basename in args exists as a .pac in the same directory
            // If it does, load it as an ARCNode
            string basename = System.IO.Path.GetFileNameWithoutExtension(args[2]);
            string pacPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(args[2]), basename + ".pac");
            if (!File.Exists(pacPath))
            {
                // Copy the local (cwd) .pac file as a template
                string localPacPath = System.IO.Path.Combine(Environment.CurrentDirectory, "template.pac");
                if (!File.Exists(localPacPath))
                {
                    Console.WriteLine("No .pac file found in {0} or {1}.", System.IO.Path.GetDirectoryName(args[2]), Environment.CurrentDirectory);
                    return;
                }
                File.Copy(localPacPath, pacPath);
                Console.WriteLine("Copied {0} to {1}.", localPacPath, pacPath);
            }
            ARCNode stage = NodeFactory.FromFile(null, pacPath) as ARCNode;

            // Find first child with children (the first one usually doesn't have any)
            ARCNode stageEntry = null;
            foreach (ARCNode entry in stage.Children)
            {
                if (entry.Children.Count > 0)
                {
                    stageEntry = entry;
                    break;
                }
            }

            // Error if not found
            if (stageEntry == null)
            {
                Console.WriteLine("No stage entries found in {0}.", pacPath);
                return;
            }

            BRRESNode modelData = null;
            foreach (ARCEntryNode entry in stageEntry.Children)
            {
                if (entry.isModelData())
                {
                    modelData = entry as BRRESNode;
                    break;
                }
            }

            // Error if not found
            if (modelData == null)
            {
                Console.WriteLine("No model data found in {0}.", pacPath);
                return;
            }

            // Find the 3DModels group
            BRESGroupNode modelsGroup = null;
            foreach (BRESGroupNode group in modelData.Children)
            {
                if (group.Name == "3DModels(NW4R)")
                {
                    modelsGroup = group;
                    break;
                }
            }

            // Error if not found
            if (modelsGroup == null)
            {
                Console.WriteLine("No 3DModels group found in {0}.", pacPath);
                return;
            }

            // Find the first MDL0Node
            MDL0Node model = null;
            foreach (MDL0Node node in modelsGroup.Children)
            {
                model = node;
                break;
            }

            // Error if not found
            if (model == null)
            {
                Console.WriteLine("No MDL0Node found in {0}.", pacPath);
                return;
            }

            Console.WriteLine("Opening Blender...");

            string vmfPath = args[2];
            string colladaPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(args[2]), basename + ".dae");

            // Script path is CWD
            string colladaScriptPath = System.IO.Path.Combine(Environment.CurrentDirectory, "ColladaExport.py");

            // Execute blender --background --python path/to/executable/ColladaExport.py -- <vmfPath> <colladaPath>
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "blender",
                Arguments = string.Format("--background --python \"{0}\" -- \"{1}\" \"{2}\"", colladaScriptPath, vmfPath, colladaPath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo = startInfo
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            process.WaitForExit();

            // Error if not found
            if (!File.Exists(colladaPath))
            {
                Console.WriteLine("Collada model could not be generated.");
                return;
            }
            MDL0Node colladaModel = new Collada {Text = $"Import Settings - {System.IO.Path.GetFileName(colladaPath)}"}.ImportModel(colladaPath,
                    Collada.ImportType.MDL0) as MDL0Node;
            
            // Replace the model with the collada model
            model.Replace(colladaModel);

            // Delete the collada model
            colladaModel.Dispose();
            File.Delete(colladaPath);

            Console.WriteLine("Model replaced.");

            // Find the texture data node
            BRRESNode textureData = null;
            foreach (ARCEntryNode entry in stageEntry.Children)
            {
                if (entry.isTextureData())
                {
                    textureData = entry as BRRESNode;
                    break;
                }
            }

            // Error if not found
            if (textureData == null)
            {
                Console.WriteLine("No texture data found in {0}.", pacPath);
                return;
            }

            // Find texture group
            BRESGroupNode texturesGroup = null;
            foreach (BRESGroupNode group in textureData.Children)
            {
                if (group.Name == "Textures(NW4R)")
                {
                    texturesGroup = group;
                    break;
                }
            }

            // Clear the texture data
            texturesGroup.Children.Clear();

            // Import textures from the working directory
            DirectoryInfo dir = new DirectoryInfo(System.IO.Path.GetDirectoryName(args[2]));
            FileInfo[] files;
            files = dir.GetFiles();
            foreach (FileInfo info in files)
            {
                string ext = System.IO.Path.GetExtension(info.FullName).ToUpper();
                if (ext == ".PNG" || ext == ".TGA" || ext == ".BMP" || ext == ".JPG" || ext == ".JPEG" ||
                    ext == ".GIF" || ext == ".TIF" || ext == ".TIFF")
                {
                    // Convert the texture
                    TextureConverter format = TextureConverter.Get(WiiPixelFormat.RGB565);
                    Bitmap bitmap = new Bitmap(info.FullName);
                    FileMap textureFile = format.EncodeTEX0Texture(bitmap, 1);
                    bitmap.Dispose();
                    TEX0Node texture = textureData.CreateResource<TEX0Node>(System.IO.Path.GetFileNameWithoutExtension(info.FullName));
                    // Add the texture to the texture group
                    texture.ReplaceRaw(textureFile);
                    texturesGroup.AddChild(texture);
                    // Delete the file
                    info.Delete();
                }
            }

            Console.WriteLine("Textures imported.");

            // Save the stage
            stage.Rebuild();
            model.Rebuild();
            modelsGroup.Rebuild();
            modelData.Rebuild();
            texturesGroup.Rebuild();
            textureData.Rebuild();
            stageEntry.Rebuild();
            stage.Rebuild();
            stage.Export(pacPath);

            Console.WriteLine("Stage saved to {0}.", pacPath);
        }
    }
}
