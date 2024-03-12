# References

## Studio

- [x] base class: `StudioReference`
- [x] derived classes for each reference type (asset, script, folder, etc)
- [x] contains only the GUID of referenced Project Node
- [x] references to scripts are limited to scripts generated and managed by studio
- [x] generic references
- [ ] when imported to **Resources** it's translated to relative path

## Engine

- [x] base class: `Reference`
- [x] derived classes for each reference type
- [x] contains only the Relative path from **Resources** folder
- [ ] referenced asset can be loaded by calling method of reference
- [ ] recursive load of definition assets managed by Studio
- [x] script references are not supported

## Editing

- [x] reference control that consists of:
  - [x] pick button
  - [x] name of referenced object
  - [x] reference type
- [x] reference can be picked by filtered list of assets
- [ ] reference can be set by drag&drop from project

## Inspector changes

References are dependent on studio specific classes and model.
Inspector is (and should be) studio independent.
To support reference control, new derived class was added `StudioInspector`.

## Architecture changes

There are just too many cases where an access to `StudioModel` is needed.
Many small classes need it, so I've decided to make a **singleton** from `StudioModel`.

## Result

❌ References were replaced by data classes and c# references.