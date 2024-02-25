# Prefabs

## Goal
Ideal case: prefab referenced by asset database or directly in code can be instantiated to world with all its hierarchy by single call.

## Options
Let's consider following scenarios:
- A: I need to adjust camera to fit certain position and angle, based on pixel art layer
- B: I need to create 3D metadata about collisions and space occupancy around asset

### No prefabs
- ✅ nothing to do, it's already "done"
- ❌ manual setup - easy to break
- ❌ hard to position anything in 3D space
- ❌ scales badly - more prefabs means much more work
- A: impossible - other tool needed
- B: hard

### Blender
- ✅ fully working 3D tool
- ❌ questionable custom data
- ❌ custom data needs to pass through FBX and Assimp
- ❌ part of pipeline has to be created in Blender and needs to be maintained with Blender upgrades
- ❌ how to reference resources?
- A: hard/impossible - camera in blender will be different to the game
- B: hard - with some helper sub-meshes and color coding 

### Fully featured editor
- ✅ exactly what I want to do
- ❌ extremely heavy workload
- ❌ heavyweight with all disadvantages the Unity has
- A: easy
- B: easy

### Hand written XML
- ✅ no special editor needed
- ❌ manual maintenance and migration
- ❌ hard to tweak in 3D space
- ❌ how to reference resources?
- A: impossible - other tool needed
- B: hard

### Hand written XML with Studio tweaking
- ✅ no special editor needed
- ✅ relatively light-weight tool in studio for position tweaking
- ❌ manual maintenance and migration
- ❌ save/serialization after tweaking will change formatting of XML
- ❌ how to reference resources?
- A: possible
- B: medium - needs to be written outside but there is visualization of some kind

### Partial editor
- ✅ editing in 3D space without unity problems
- ❌ needs universal editor for custom components
- ❌ still a lot of work
- ❌ how to reference resources?
- A: possible
- B: easy

### Code based prefabs generated from studio
- ✅ editing in 3D space without unity problems
- ✅ no loading needed
- ✅ natural resource references
- ❌ needs universal editor for custom components
- ❌ still a lot of work
- ❌ manual maintenance of game components
- ❌ problematic deserialization in studio
- A: possible
- B: easy

### Combination of hand written hierarchy and 3D tweaks in single XML file
- ✅ no special editor needed
- ✅ relatively light-weight tool in studio for position tweaking
- ✅ save/serialization will modify just the "data" part of the file
- ❌ manual maintenance and migration
- ❌ harder to match, 3D data with written structure, but possible
- ❌ how to reference resources?

- A: possible
- B: medium - needs to be written outside but there is visualization of some kind

### Combination of hand written hierarchy and 3D tweaks in single C# file
- ✅ no special editor needed
- ✅ relatively light-weight tool in studio for position tweaking
- ✅ save/serialization will modify just the "data" part of the file
- ✅ natural resource references
- ❌ manual maintenance and migration
- ❌ harder to match, 3D data with written structure, but possible
- ❌ how to parse hierarchy from source?

- A: possible
- B: medium - needs to be written outside but there is visualization of some kind

### Unity
- ❌ NO
