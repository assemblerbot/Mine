namespace GameToolkit.Framework;

public sealed class Scene : IDisposable
{
	private List<GameObject> _gameObjects = new();

	private readonly SortedList<IUpdatable, int>  _updatables  = new();
	private readonly SortedList<IRenderable, int> _renderables = new();
	
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
		foreach(KeyValuePair<IUpdatable, int> updatablePair in _updatables)
		{
			updatablePair.Key.Update(timeDelta);
		}
	}

	public void RegisterUpdatable(IUpdatable updatable)
	{
		_updatables.Add(updatable, updatable.UpdateOrder);
	}
	
	public void UnregisterUpdatable(IUpdatable updatable)
	{
		_updatables.Remove(updatable);
	}
	#endregion

	#region Rendering
	public void Render()
	{
		foreach(KeyValuePair<IRenderable, int> renderablePair in _renderables)
		{
			renderablePair.Key.Render();
		}
	}
	
	public void RegisterRenderable(IRenderable renderable)
	{
		_renderables.Add(renderable, renderable.RenderOrder);
	}

	public void UnregisterRenderable(IRenderable renderable)
	{
		_renderables.Remove(renderable);
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