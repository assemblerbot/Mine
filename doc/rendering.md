# Rendering

`Render()` is called on renderable objects only.
Meshes and other objects are selected for rendering in renderable object `Render()` call.
Even postprocessing is type of camera that renders quad mesh with appropriate flags.


## Renderer
- camera, light, etc..
- attributes:
  - ✅ render order
  - ✅ render layer mask - 64bit
    - all objects that matches at least one bit in mask are rendered
  - clipper type
    - each renderable object can have different clipper
    - renderable object holds temporal clipping cache
    - none clipper and everything clipper are also available for special renderings
  - ✅ list of passes
    - order is important
    - ✅ name
    - clear parameters per pass
      - color, depth, or not clearing at all
    - render target per pass
      - target is resource, dynamically created
      - per pass is required for multi-pass filters and other stuff
  
## Renderable
- objects that can be rendered
- attributes:
  - layer - 64bit
  - material
  - order? .. custom sorting for transparent objects?