# World

## Hierarchy
- entities and components
- entities are in hierarchy
- world has just one fixed entity - `root`
- only components are customizable, entity is not

## Update loop
- Update on all registered components
- Render on all registered components

## Event functions

### AfterAddedToWorld
- called on component after entity was added to hierarchy

```
Entity entity = new();          // just constructor
World.Root.AddChild(entity);    // AfterAddedToWorld() called
```

```
Entity entityA = new();          // just constructor
Entity entityB = new();          // just constructor
entityA.AddChild(entityB);       // nothing
World.Root.AddChild(entityA);    // AfterAddedToWorld() called on both, entityA first
```

### BeforeRemovedFromWorld
- called on component after entity was removed from hierarchy

```
Entity entity = World.Root.FindChild(path);
World.Root.RemoveChild(entity);  // BeforeRemovedFromWorld() called
```

```
Entity entityA = new();         // just constructor
Entity entityB = new();         // just constructor
entityA.AddChild(entityB);      // nothing called
entityA.RemoveChild(entityB);   // nothing called
```

```
Entity entityA = new();         // just constructor
Entity entityB = new();         // just constructor
entityA.AddChild(entityB);      // nothing
World.Root.AddChild(entityA);   // AfterAddedToWorld() called on both, entityA first
World.Root.RemoveChild(entityA);        // BeforeRemovedFromWorld() called on both, entityB first
```

### Dispose
- called from Entity `Dispose()` function, when entity is destroyed by `DestroyChild()` function
- if entity is just removed then Dispose is NOT called

```
Entity entity = World.Root.FindChild(path);
entity.Destroy();         // BeforeRemovedFromWorld() called then Dispose()
```

### AfterEntityFlagsChanged
- called after any of entity's flags was changed
 