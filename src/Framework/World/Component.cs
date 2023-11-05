namespace GameToolkit.Framework;

public class Component
{
	protected GameObject _gameObject = null!;

	public void SetGameObject(GameObject gameObject)
	{
		_gameObject = gameObject;
	}

	public virtual void Dispose()
	{
	}
}