namespace GameToolkit.Framework;

public interface IUpdatable
{
	int UpdateOrder { get; }
	void Update(double timeDelta);
}