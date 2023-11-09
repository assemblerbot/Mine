using Silk.NET.Maths;

namespace GameToolkit.Framework;

public sealed class Scene : IDisposable
{
	private List<GameObject> _gameObjects = new();

	private readonly SortedList<int, IUpdatable>  _updatables  = new();
	private readonly SortedList<int, IRenderable> _renderables = new();

	private readonly List<IUpdatable>  _updatablesToRemove  = new();
	private readonly List<IRenderable> _renderablesToRemove = new();
	
	public void Add(GameObject gameObject)
	{
		_gameObjects.Add(gameObject);
	}

	public void Remove(GameObject gameObject)
	{
		int index = _gameObjects.IndexOf(gameObject);
		if (index == -1)
		{
			// TODO - log error
			return;
		}

		// gameObject.Disable();
		
		_gameObjects.Remove(gameObject);
	}
	
	#region Updating
	public void Update(double timeDelta)
	{
		foreach(var updatablePair in _updatables)
		{
			updatablePair.Value.Update(timeDelta);
		}

		CleanupUpdatables();
	}

	public void RegisterUpdatable(IUpdatable updatable)
	{
		_updatables.Add(updatable.GetUpdateOrder(), updatable);
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

		foreach (IUpdatable updatable in _updatablesToRemove)
		{
			int index = _updatables.IndexOfValue(updatable);
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
		foreach(var renderablePair in _renderables)
		{
			renderablePair.Value.Render();
		}
	}

	public void WindowSizeChanged(Int2 size)
	{
		foreach(var renderablePair in _renderables)
		{
			renderablePair.Value.WindowResized(size);
		}
	}

	public void RegisterRenderable(IRenderable renderable)
	{
		_renderables.Add(renderable.GetRenderOrder(), renderable);
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

		foreach (IRenderable renderable in _renderablesToRemove)
		{
			int index = _renderables.IndexOfValue(renderable);
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
		for (int i = 0; i < _gameObjects.Count; ++i)
		{
			_gameObjects[i].Dispose();
		}
		_gameObjects.Clear();
	}
}