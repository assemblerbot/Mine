# MINE engine design document

## Philosophy of the engine

- engine is a set of libraries
- engine itself is a library
- engine does nothing by itself except simple calls to scene and renderer
- every feature is optional
- every feature can be created from scratch in game code

## Game built on top of the MINE engine

- game must be a library too in order to be properly managed by Studio
- so game solution needs always two projects:
  - game library
  - game executable

## Studio

- studio is like a game built on top of engine
- studio must be able to create new game projects including it's solution and all necessary files
  - from template 
- studio must organize assets and resources through plugin manager

## Scripts

- game scripts are in `GameLibrary` directory - not in Assets
- scripts are ignored by Studio
- ~~engine connects to scripts via built game dll~~
- studio may generate scripts

## Scene organization

- there is only one scene named `World`
  - scene cannot be loaded or unloaded - only game objects it contains can be
  - scene is a tree of game objects
    - each game object has a transformation and a list of components

## Components

- each component is derived from `Component` class
- there are `Updatable`, `Renderable`, `Renderer` and `Light` components managed by engine
- all managed components needs to be registered to lists manually

## Engine features

- all engine features are implemented as components and are optional
- any feature that is not directly connected to engine libraries should be in separate package, outside of engine
- engine contains built-in components for features that are directly connected to engine libraries
  - but also these components must be added to the scene manually

## Flow of the game

- all game features are instantiated manually as game objects and components
- first game prefab is also instantiated manually
  - this prefab loads and instantiates rest of game prefabs

## Resource management

- studio resources are in `Assets` directory
	- all files has .meta file which contains:
		- GUID
		- hash
		- importers
    - in export phase `AssetDatabase` script is generated
	- prefabs can be created in studio
    - prefabs reference files via GUID or direct reference to `AssetDatabase`

- engine resources are in `Resources` directory
	- archives are zip files directly under Resources directory
		- ordered by name
		- later archive overrides structurally previous archives
		- relative path to each file is taken from root inside archive
	- files are in subfolders
		- files overrides content of archives
		- relative path from Resources directory
	- ~~prefabs reference files via relative resource path (needs to be changed from GUID in build phase)~~

## Prefabs OBSOLETE

- prefabs are:
  - edited in studio
  - in Assets directory
  - json files with extension **.prefab**
  - loaded by engine and instantiated
  - referenced by guid in editor and by relative path in engine
- prefab can be loaded and instantiated without adding to scene
  - this is something like scriptable object in Unity 

## Prefabs

- prefabs are:
  - created from scenes in studio
  - script classes in a scripts directory
  - updated by studio if mesh changes ?
  - instantiated by standard calls from c#

## Scenes

- scene is hierarchy of meshes loaded from any format supported by Assimp
- stored as hierarchy in `Resources`
- contains meshes, transform data and additional info (materials, etc) 

## Meshes OBSOLETE

- loaded from any mesh file that is supported by Assimp
- stored in engine resources as binary json files with **.mesh** extension
- or in original format, so they can be loaded via Assimp again in engine
  - this is useful for modding
- mesh component holds a reference to mesh resource and a list of materials

## Textures

- loaded from any image file that is supported by ??? (TODO - needs research)
- stored in engine resources as binary json files with **.image** extension
- or in original format, so they can be loaded via ??? again in engine
  - this is useful for modding

## Shaders

- created externally
- compiled by third-party tool from studio in import phase
- bytecode stored in Resources

## Materials OBSOLETE

- materials are created in studio
- material is:
  - reference to a shader
  - list of textures
  - list of shader parameters

## Material definitions

- C# classes created manually
- can reference anything from `AssetDatabase` or any other part of code

## Materials

- instances of material definitions in C#

## Camera

- basic camera function is following:
  - is selects renderable objects to render
    - by frustum culling
    - by layers / tags ?
    - by distance
    - by occlusion culling (any algorithm - cell, portal, etc.)
  - calls single render pass on selected objects
    - pass is adjustable in studio as one of renderable parameters
  - output goes to a render target or screen (configurable)
  - a typical renderables are:
    - camera
    - light
    - post process effect

## Physics

- physics component is a rigid body or any physics related object
- physics component updates transformation of game object it is attached to
- physics component is updated in constant time steps
- there is one component - physics world, that is responsible for physics simulation

## Audio

- audio component is a sound source or listener
- listener may use similar approach as renderables to select sound sources
- audio assets are imported from any supported format by ??? (TODO - needs research)
- audio assets are stored in Resources

## UI

- UI is rendered as a separate render pass
- all UI objects are rendered by UI renderable component that must be attached to a game object
- ImGui is optional and it's implemented as built-in component

### Plugins

- plugin is combination of scripts and resources
- plugin manager have to copy:
  - scripts to `GameLibrary/Plugins` folder
  - resources to `Assets/Plugins` folder
- reverse process - updating of the plugin - needs to combine those two folders into one plugin folder
- plugin json and possible descriptors/meta files/etc sould be together with scripts
