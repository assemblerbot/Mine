namespace GameToolkit.Framework;

public interface IUpdatable
{
	int GetUpdateOrder();
	void Update(double timeDelta);
}