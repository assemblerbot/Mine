namespace GameToolkit.Framework;

public sealed class Scene : IDisposable
{
	private List<GameObject> _gameObjects = new();

	private List<IUpdatable> _updatables = new();
	
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

	public void Dispose()
	{
		for (int i = 0; i < _gameObjects.Count; ++i)
		{
			_gameObjects[i].Dispose();
		}
		_gameObjects.Clear();
	}
}