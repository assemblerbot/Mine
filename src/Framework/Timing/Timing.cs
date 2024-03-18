namespace Mine.Framework;

public sealed class Timing
{
	private ulong _updateFrame = 0;
	private ulong _renderFrame = 0;
	
	public readonly DateTime Start = DateTime.Now;
	
	public          ulong    UpdateFrame => _updateFrame;
	public          ulong    RenderFrame => _renderFrame;
	
	public void UpdateCalled()
	{
		++_updateFrame;
	}

	public void RenderCalled()
	{
		++_renderFrame;
	}
}