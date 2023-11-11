using GameToolkit.Framework;

namespace GameToolkit.Studio;

public sealed class StudioComponent : Component, IUpdatable
{
	public int GetUpdateOrder() => 0;

	public void Update(double timeDelta)
	{
		
	}

	public override void Dispose()
	{
		
	}
}