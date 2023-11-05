# Mine
MINimalistic Engine - MINE

## Components

Components are as lite as possible. Only `Dispose` is called on them by default.

### Interfaces

`IUpdatable`
- component can be registered for update calls

`IRenderable`
- component can be registered for render calls
- typical components of this type are: camera, light
- note: meshes and other renderable objects are not the ones Render should be called on

