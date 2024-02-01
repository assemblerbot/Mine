namespace Mine.Framework;

public interface IUpdatable
{
	Entity Entity { get; }

	int GetUpdateOrder();
	void Update(double timeDelta);
}