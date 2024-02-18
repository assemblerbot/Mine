namespace Mine.Framework;

public interface IUpdatable
{
	Entity Entity { get; }
	int UpdateOrder { get; }
	
	void Update(double timeDelta);
}