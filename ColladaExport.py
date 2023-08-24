# Blender script by KiwifruitDev
# Import arg 1 as "Valve Map Format (.vmf)" and export to arg 2 as "Collada (.dae)"

import bpy
import sys
import os
from io_import_vmf import import_vmf
from vmfpy.fs import VMFFileSystem

# Get the arguments
argv = sys.argv
argv = argv[argv.index("--") + 1:]

# Get the file paths
inputPath = argv[0]
outputPath = argv[1]

# Delete the default cube if it exists
if bpy.data.objects.get("Cube") is not None:
    bpy.data.objects["Cube"].select_set(True)
    bpy.ops.object.delete()

# Delete the default light if it exists
if bpy.data.objects.get("Light") is not None:
    bpy.data.objects["Light"].select_set(True)
    bpy.ops.object.delete()

# Delete the default camera if it exists
if bpy.data.objects.get("Camera") is not None:
    bpy.data.objects["Camera"].select_set(True)
    bpy.ops.object.delete()

# Import the VMF
data_dirs = [
    'C:\Program Files (x86)\Steam\steamapps\common\Source SDK Base 2013 Multiplayer\hl2',
    'C:\Program Files (x86)\Steam\steamapps\common\Source SDK Base 2013 Multiplayer\hl2\materials',
]
data_paks = []

fs = VMFFileSystem(data_dirs, data_paks, index_files=True)

importer = import_vmf.VMFImporter(  fs, None,
                                    import_solids=True, import_overlays=False,
                                    import_props=False, optimize_props=False,
                                    skip_collision=True, skip_lod=False,
                                    import_sky_origin=False,
                                    import_sky=False, sky_resolution=512,
                                    import_materials=True,
                                    simple_materials=True, cull_materials=True,
                                    texture_interpolation='Linear',
                                    editor_materials=False,
                                    reuse_old_materials=False,
                                    reuse_old_models=False,
                                    import_lights=False,
                                    scale=1.0, epsilon=0.0001,
                                    light_factor=1.0, sun_factor=1.0,
                                    ambient_factor=1.0,
                                    verbose=False,
                                    skip_tools=False,
                                    separate_tools=False,
                                    blend_use_vertex_alpha=False)

with importer:
    importer.load(inputPath, bpy.context, None)

# Save as DAE (Y up, -Z forward)
bpy.ops.wm.collada_export(filepath=outputPath, check_existing=False, export_global_forward_selection='-Z', export_global_up_selection='Y', apply_global_orientation=True)

# Exit Blender
bpy.ops.wm.quit_blender()

                                  