namespace GameToolkit.Framework;

public class Component
{
	protected Scene      Scene => Engine.Scene;

	private GameObject _gameObject = null!;
	public GameObject GameObject => _gameObject;

	public void SetGameObject(GameObject gameObject)
	{
		_gameObject = gameObject;
	}

	// 'event' functions
	public virtual void OnInstantiate()
	{
	}

	public virtual void AfterAddedToScene()
	{
	}
	
	public virtual void BeforeRemovedFromScene()
	{
	}

	public virtual void Dispose()
	{
	}
}