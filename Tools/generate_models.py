"""Generate the game's original, simple Blender models. Run with Blender -b -P."""
import os
import bpy

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUTPUT = os.path.join(ROOT, "Assets", "Resources", "Models")
os.makedirs(OUTPUT, exist_ok=True)


def reset():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)


def box(name, location, dimensions, bevel=0.06):
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    obj = bpy.context.object
    obj.name = name
    obj.dimensions = dimensions
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    modifier = obj.modifiers.new("Soft edges", "BEVEL")
    modifier.width = bevel
    modifier.segments = 3
    bpy.ops.object.modifier_apply(modifier=modifier.name)
    for face in obj.data.polygons:
        face.use_smooth = True
    modifier = obj.modifiers.new("Weighted normals", "WEIGHTED_NORMAL")
    bpy.ops.object.modifier_apply(modifier=modifier.name)
    return obj


def export(name):
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.export_scene.fbx(filepath=os.path.join(OUTPUT, name + ".fbx"),
                             use_selection=True, object_types={"MESH"},
                             axis_forward="-Z", axis_up="Y", add_leaf_bones=False)


def gable():
    mesh = bpy.data.meshes.new("RoofMesh")
    vertices = [(-.78,-.65,1.24),(.78,-.65,1.24),(-.78,.65,1.24),(.78,.65,1.24),
                (0,-.65,1.85),(0,.65,1.85)]
    mesh.from_pydata(vertices, [], [(0,1,4),(2,5,3),(0,4,5,2),(1,3,5,4),(0,2,3,1)])
    mesh.update()
    obj = bpy.data.objects.new("Roof", mesh)
    bpy.context.collection.objects.link(obj)


def cylinder(name, location, radius, depth, vertices=10):
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius, depth=depth, location=location)
    bpy.context.object.name = name
    return bpy.context.object


reset()
box("Wall", (0, 0, 0.65), (1.25, 1.05, 1.3), 0.09)
gable()
box("Door", (0, -0.536, 0.36), (0.32, 0.06, 0.64), 0.04)
box("Window_Left", (-0.41, -0.536, 0.83), (0.23, 0.06, 0.25), 0.035)
box("Window_Right", (0.41, -0.536, 0.83), (0.23, 0.06, 0.25), 0.035)
box("Chimney", (0.43, 0.20, 1.68), (0.20, 0.20, 0.55), 0.035)
box("Foundation", (0, 0, 0.09), (1.38, 1.18, 0.18), 0.04)
box("Doorframe", (0, -0.57, 0.38), (0.40, 0.05, 0.74), 0.03)
export("VillageHouse")

reset()
box("Base", (0, 0, 0.09), (0.9, 0.7, 0.18), 0.05)
box("Body", (0, 0, 0.48), (0.58, 0.48, 0.7), 0.10)
box("Cap", (0, 0, 0.87), (0.70, 0.58, 0.16), 0.07)
box("Pipe", (0, 0.35, 0.35), (0.22, 0.48, 0.22), 0.06)
box("Gauge", (0, -0.25, 0.57), (0.24, 0.06, 0.22), 0.04)
cylinder("Valve", (0, 0.36, 0.52), 0.18, 0.09, 12)
box("Outlet", (0, 0.49, 0.22), (0.28, 0.20, 0.35), 0.05)
export("WaterPump")

reset()
box("Concrete", (0, 0, 2.1), (0.8, 5.9, 4.2), 0.18)
box("Crest", (0, 0, 4.22), (1.05, 6.0, 0.24), 0.08)
for y in (-2.25, 0, 2.25):
    box("Buttress", (0.50, y, 1.35), (0.55, 0.55, 2.7), 0.08)
box("Spillway", (0.44, 0, 2.5), (0.12, 1.4, 2.5), 0.04)
export("Dam")

reset()
cylinder("Trunk", (0, 0, 0.65), 0.16, 1.3, 9)
for z, radius in ((1.15, .72), (1.65, .58), (2.05, .42)):
    bpy.ops.mesh.primitive_cone_add(vertices=9, radius1=radius, radius2=0.08, depth=1.0, location=(0, 0, z))
    bpy.context.object.name = "Foliage"
export("PineTree")

reset()
bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1, radius=1, location=(0, 0, 0.4))
bpy.context.object.name = "Rock"
bpy.context.object.scale = (0.8, 0.65, 0.45)
bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
export("Rock")

reset()
box("Grass", (0, 0, -0.25), (20, 8, 0.5), 0.22)
export("Terrain")
