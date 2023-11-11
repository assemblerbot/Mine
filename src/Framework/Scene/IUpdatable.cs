namespace GameToolkit.Framework;

public interface IUpdatable
{
	GameObject GameObject { get; }

	int GetUpdateOrder();
	void Update(double timeDelta);
}