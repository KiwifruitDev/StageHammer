# Blender script by KiwifruitDev
# Import arg 1 as "Valve Map Format (.vmf)" and export to arg 2 as "Collada (.dae)"

import bpy
import sys
import os
from plumber.plumber import Importer, FileSystem
from plumber.asset import AssetCallbacks

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
rootgame = "D:\SteamLibrary\steamapps\common\Source SDK Base 2013 Multiplayer"
game = "hl2"

fs = FileSystem("HALF-LIFE 2", [
    ("WILDCARD", rootgame + "/" + game + "/custom"),
    ("VPK", rootgame + "/" + game + "/hl2_pak.vpk"),
    ("VPK", rootgame + "/" + game + "/hl2_textures.vpk"),
    ("VPK", rootgame + "/" + game + "/hl2_misc.vpk"),
    ("DIR", rootgame + "/" + game),
])

asset_callbacks = AssetCallbacks(
    bpy.context,
    main_collection=None,
    brush_collection=None,
    overlay_collection=None,
    prop_collection=None,
    light_collection=None,
    entity_collection=None,
    apply_armatures=False,
)

try:
    importer = Importer(
        fs,
        asset_callbacks,
        3,
        import_lights=False,
        light_factor=0,
        sun_factor=0,
        ambient_factor=0,
        import_sky_camera=False,
        sky_equi_height=None,
        import_unknown_entities=False,
        scale=1,
        target_fps=24,
        remove_animations=True,
        simple_materials=True,
        allow_culling=True,
        editor_materials=False,
        texture_interpolation="Linear",
        # automatic map data path detection happens here
        vmf_path=inputPath,
        map_data_path="",
    )
except OSError as err:
    print(f"Could not parse vmf: {err}")

try:
    importer.import_vmf(
        inputPath,
        False,
        import_brushes=True,
        import_overlays=True,
        epsilon=0.001,
        cut_threshold=0.001,
        merge_solids="SEPARATE",
        invisible_solids="SKIP",
        import_materials=True,
        import_props=True,
        import_entities=False,
        import_sky=False,
        scale=1,
    )
except OSError as err:
    print(f"Could not import vmf: {err}")

asset_callbacks.finish()

# Select all objects
bpy.ops.object.select_all(action='SELECT')

# Remove all armature modifiers
for obj in bpy.context.selected_objects:
    for modifier in obj.modifiers:
        if modifier.type == "ARMATURE":
            obj.modifiers.remove(modifier)
    # Apply position/rotation to geometry
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)

# Remove all armatures
for armature in bpy.data.armatures:
    bpy.data.armatures.remove(armature)

# Rename all material slot names without prefixed path
for material in bpy.data.materials:
    # Get last slash index
    lastSlash = material.name.rfind("/")
    # Get the material name
    materialName = material.name[lastSlash + 1:]
    # Set the material name
    material.name = materialName

# Rename all image names without prefixed path
for image in bpy.data.images:
    # Ignore Render Result and Viewer Node
    if image.name == "Render Result" or image.name == "Viewer Node":
        continue
    # Get last slash index
    lastSlash = image.name.rfind("/")
    # Get the image name
    imageName = image.name[lastSlash + 1:]
    # Set the image name
    image.name = imageName

# Save as DAE (Y up, -Z forward)
bpy.ops.wm.collada_export(filepath=outputPath,
    check_existing=False,
    export_global_forward_selection='-Z',
    export_global_up_selection='Y',
    apply_global_orientation=True,
)

# Exit Blender
bpy.ops.wm.quit_blender()
