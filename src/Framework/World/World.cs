namespace Mine.Framework;

public sealed class World : IDisposable
{
	public readonly Entity Root = new();

	private readonly ManagedList<IUpdatable> _updatables = new(); // sorted by update order
	private readonly ManagedList<IRenderer>  _renderers  = new(); // sorted by render order

	private readonly List<MeshComponent>          _meshes = new();
	public           IReadOnlyList<MeshComponent> Meshes => _meshes;

	private readonly List<LightComponent>          _lights = new();
	public           IReadOnlyList<LightComponent> Lights => _lights;

	public World()
	{
		Root.SetFlags(EntityFlags.IsInWorld);
	}

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
	public void RegisterMesh(MeshComponent mesh)
	{
		_meshes.Add(mesh);
	}

	public void UnregisterMesh(MeshComponent mesh)
	{
		_meshes.Remove(mesh);
	}
	
	//---
	public void RegisterLight(LightComponent light)
	{
		_lights.Add(light);
	}

	public void UnregisterLight(LightComponent light)
	{
		_lights.Remove(light);
	}
	#endregion
	
	public void Dispose()
	{
		Root.Dispose();
	}
}