# StageHammer
Use Hammer to create Super Smash Bros. Brawl stages.

## Requirements
- [.NET 4.7.2 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net472-web-installer)
- Source SDK Base 2013 Multiplayer (An installation of [Hammer++](https://ficool2.github.io/HammerPlusPlus-Website/) is recommended)
- [Blender](https://www.blender.org/download/) (Must be added to the PATH environment variable)
- [io_import_vmf](https://github.com/lasa01/io_import_vmf/) installed and enabled in Blender
- A stage *.pac file (You must either rip this from the game or set up a new stage in [BrawlCrate](https://github.com/soopercool101/BrawlCrate))

## Usage
1. [Download the latest release](https://github.com/KiwifruitDev/StageHammer/releases/latest/download/stagehammer.zip)
1. Extract the zip file
1. Place your stage *.pac file in the same folder as `stagehammer.exe` and rename it to `template.pac`
1. Open Hammer(++) from Source SDK Base 2013 Multiplayer (Use `Half-Life 2` preset)
1. Go to `Tools` > `Options` > `Build Programs`
1. Set `BSP Executable` to the path of `stagehammer.exe`
1. (Optional) Create material (VMT) and texture (VTF) files within the root folder in `hl2\materials` for your stage
1. Create a new map (VMF)
1. Add at least one piece of geometry to the map
1. Save the map (If you are actively developing a mod, save this under a stage name to `[MOD DATA]\stage\adventure` or `[MOD DATA]\stage\melee`)
1. Go to `File` > `Run Map...`
1. Select `Normal` for the `Run BSP` option
1. Select `No` for the `Run VIS` option
1. Select `No` for the `Run RAD` option
1. Select the `Don't run the game after compiling` and (optionally) `Wait for keypress after compiling` options
1. Click `OK` to compile the map
1. If you have `Wait for keypress after compiling` enabled, press any key to continue

## Notes
- Materials (and their textures) from VPK files will not be parsed. You must either create new material (VMT) and texture (VTF) files or copy them from the VPK files to the `hl2\materials` folder.
- It is recommended to avoid placing materials and textures in subfolders within the `hl2\materials` folder.

## License
This software is licensed under the MIT License. See [LICENSE](LICENSE) for more information.

## Third-Party Licenses
This software uses the following third-party libraries:

### [Sledge.Formats.Map](https://github.com/LogicAndTrick/sledge-formats)

```
MIT License

Copyright (c) 2019 Daniel Walder

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### [BrawlCrateLib](https://github.com/soopercool101/BrawlCrate)

```
The MIT License (MIT)

Copyright (c) 2019 soopercool101. BrawlBox, BrawlLib, BrawlTools (c) 2014 libertyernie

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```