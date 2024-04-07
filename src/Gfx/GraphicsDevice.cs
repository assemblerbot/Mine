namespace Gfx;

/// <summary>
/// An object encapsulating functionality of underlying native graphics device object.
/// See <see cref="GraphicsDeviceOptions"/>.
/// </summary>
public abstract class GraphicsDevice : IDisposable
{
	public virtual void Dispose()
	{
		
	}

	public void ResizeMainWindow(uint x, uint y)
	{
	}

	public void WaitForIdle()
	{
	}

	public void SwapBuffers()
	{
	}
}