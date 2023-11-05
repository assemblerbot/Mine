namespace GameToolkit.Framework;

public sealed class Scene : IDisposable
{
	private List<GameObject> _gameObjects = new();

	private readonly List<IUpdatable>  _updatables  = new();
	private readonly List<IRenderable> _renderables = new();
	
	public void Update(double timeDelta)
	{
		for(int i=0;i<_updatables.Count;i++)
		{
			_updatables[i].Update(timeDelta);
		}
	}

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
	public void RegisterUpdatable(IUpdatable updatable)
	{
		_updatables.Add(updatable);
	}
	
	public void UnregisterUpdatable(IUpdatable updatable)
	{
		int index = _updatables.IndexOf(updatable);
		if (index == -1)
		{
			// TODO - log error
			return;
		}

		_updatables.RemoveAt(index);
	}
	#endregion

	#region Rendering
	public void Render()
	{
		for (int i = 0; i < _renderables.Count; ++i)
		{
			_renderables[i].Render();
		}
	}
	
	public void RegisterRenderable(IRenderable renderable)
	{
		_renderables.Add(renderable);
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