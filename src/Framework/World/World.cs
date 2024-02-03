namespace Mine.Framework;

public sealed class World : IDisposable
{
	private List<Entity> _entities = new();

	private readonly List<IUpdatable>  _updatables  = new();	// sorted by update order
	private readonly List<IRenderable> _renderables = new();	// sorted by render order

	private readonly List<IUpdatable>  _updatablesToRemove  = new();
	private readonly List<IRenderable> _renderablesToRemove = new();
	
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
	#endregion
	
	#region Updating
	public void Update(double timeDelta)
	{
		for(int i=0;i<_updatables.Count;++i)
		{
			if (_updatables[i].Entity.ActiveInHierarchy)
			{
				_updatables[i].Update(timeDelta);
			}
		}

		CleanupUpdatables();
	}

	public void RegisterUpdatable(IUpdatable updatable)
	{
		int order = updatable.GetUpdateOrder();
		int index = _updatables.FindInsertionIndexBinary(x => x.GetUpdateOrder().CompareTo(order));
		_updatables.Insert(index, updatable);
	}
	
	public void UnregisterUpdatable(IUpdatable updatable)
	{
		_updatablesToRemove.Add(updatable);
	}

	private void CleanupUpdatables()
	{
		if (_updatablesToRemove.Count == 0)
		{
			return;
		}

		for(int i=0;i<_updatablesToRemove.Count; ++i)
		{
			int index = _updatables.IndexOf(_updatablesToRemove[i]);
			if (index == -1)
			{
				continue;
			}

			_updatables.RemoveAt(index);
		}

		_updatablesToRemove.Clear();
	}
	#endregion

	#region Rendering
	public void Render()
	{
		for(int i=0;i<_renderables.Count; ++i)
		{
			if(_renderables[i].Entity.ActiveInHierarchy)
			{
				_renderables[i].Render();
			}
		}
	}

	public void WindowSizeChanged(Vector2Int size)
	{
		for(int i=0;i<_renderables.Count; ++i)
		{
			_renderables[i].WindowResized(size);
		}
	}

	public void RegisterRenderable(IRenderable renderable)
	{
		int order = renderable.GetRenderOrder();
		int index = _renderables.FindInsertionIndexBinary(x => x.GetRenderOrder().CompareTo(order));
		_renderables.Insert(index, renderable);
	}

	public void UnregisterRenderable(IRenderable renderable)
	{
		_renderablesToRemove.Add(renderable);
	}

	private void CleanupRenderables()
	{
		if (_renderablesToRemove.Count == 0)
		{
			return;
		}

		for(int i=0;i<_renderablesToRemove.Count;++i)
		{
			int index = _renderables.IndexOf(_renderablesToRemove[i]);
			if (index == -1)
			{
				continue;
			}

			_renderables.RemoveAt(index);
		}

		_renderablesToRemove.Clear();
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