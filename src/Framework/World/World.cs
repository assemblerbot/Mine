namespace Mine.Framework;

public sealed class World : IDisposable
{
	private List<Entity> _entities = new();

	private readonly ManagedList<IUpdatable> _updatables  = new(); // sorted by update order
	private readonly ManagedList<IRenderer>  _renderers   = new(); // sorted by render order
	private readonly List<IMesh>       _meshes = new();
	private readonly List<ILight>            _lights      = new();

	public IReadOnlyList<IMesh> Meshes => _meshes;
	public IReadOnlyList<ILight>      Lights      => _lights;
	
	#region Add/Remove
	public void Add(Entity entity)
	{
		_entities.Add(entity);
		entity.AfterAddedToWorld();
	}

	public void Remove(Entity entity)
	{
		int index = _entities.IndexOf(entity);
		if (index == -1)
		{
			return;
		}

		entity.BeforeRemovedFromWorld();
		_entities.Remove(entity);
	}

	public Entity? Instantiate(SceneReference? sceneReference, Entity? parent = null)
	{
		if (sceneReference is null)
		{
			return null;
		}

		Scene? scene = sceneReference.Value;
		if (scene is null || scene.Root is null)
		{
			return null;
		}

		return InstantiateRecursiveRoot(scene.Root, scene.Root.Translation.Point3, scene.Root.Rotation, scene.Root.Scale, parent);
	}

	public Entity? Instantiate(SceneReference? sceneReference, Point3Float localPosition, QuaternionFloat localRotation, Vector3Float localScale, Entity? parent = null)
	{
		if (sceneReference is null)
		{
			return null;
		}

		Scene? scene = sceneReference.Value;
		if (scene is null || scene.Root is null)
		{
			return null;
		}

		return InstantiateRecursiveRoot(scene.Root, localPosition, localRotation, localScale, parent);
	}
	
	private Entity InstantiateRecursiveRoot(SceneNode sceneNode, Point3Float localPosition, QuaternionFloat localRotation, Vector3Float localScale, Entity? parent)
	{
		Entity entity = new (sceneNode.Name);
		if (parent is null)
		{
			Add(entity);
		}
		else
		{
			entity.SetParent(parent);
		}

		entity.LocalPosition = localPosition;
		entity.LocalRotation = localRotation;
		entity.LocalScale    = localScale;

		if (sceneNode.Children is null)
		{
			return entity;
		}

		foreach (SceneNode child in sceneNode.Children)
		{
			InstantiateRecursive(child, entity);
		}

		return entity;
	}

	private void InstantiateRecursive(SceneNode sceneNode, Entity parent)
	{
		Entity entity = new (sceneNode.Name);
		entity.SetParent(parent);

		entity.LocalPosition = sceneNode.Translation.Point3;
		entity.LocalRotation = sceneNode.Rotation;
		entity.LocalScale    = sceneNode.Scale;

		if (sceneNode.Children is null)
		{
			return;
		}

		foreach (SceneNode child in sceneNode.Children)
		{
			InstantiateRecursive(child, entity);
		}
	}
	#endregion
	
	#region Updating
	public void Update(double timeDelta)
	{
		_updatables.ForEach(x => x.Update(timeDelta));
	}

	public void RegisterUpdatable(IUpdatable updatable)
	{
		_updatables.Insert(updatable, x => x.UpdateOrder);
	}
	
	public void UnregisterUpdatable(IUpdatable updatable)
	{
		_updatables.Remove(updatable);
	}
	#endregion

	#region Rendering
	public void Render()
	{
		_renderers.ForEach(x => x.Render());
	}

	public void WindowSizeChanged(Vector2Int size)
	{
		_renderers.ForEach(x => x.WindowResized(size));
	}

	public void RegisterRenderer(IRenderer renderer)
	{
		_renderers.Insert(renderer, x => x.RenderOrder);
	}

	public void UnregisterRenderer(IRenderer renderer)
	{
		_renderers.Remove(renderer);
	}

	//---
	public void RegisterMesh(IMesh mesh)
	{
		_meshes.Add(mesh);
	}

	public void UnregisterMesh(IMesh mesh)
	{
		_meshes.Remove(mesh);
	}
	
	//---
	public void RegisterLight(ILight light)
	{
		_lights.Add(light);
	}

	public void UnregisterLight(ILight light)
	{
		_lights.Remove(light);
	}
	#endregion
	
	public void Dispose()
	{
		for (int i = 0; i < _entities.Count; ++i)
		{
			_entities[i].BeforeRemovedFromWorld();
			_entities[i].Dispose();
		}
		_entities.Clear();
	}
}