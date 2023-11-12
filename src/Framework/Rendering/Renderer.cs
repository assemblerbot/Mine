using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;

/*

 https://veldrid.dev/articles/getting-started/getting-started-part2.html

 */

namespace Mine.Framework;

public class Renderer : IDisposable
{
	private GraphicsDevice  _device;
	private ResourceFactory _factory;

	public GraphicsDevice  Device  => _device;
	public ResourceFactory Factory => _factory;
	
	public Renderer(IView view, GraphicsBackend graphicsBackend)
	{
		_device = view.CreateGraphicsDevice(
			new GraphicsDeviceOptions
			{
				// TODO - customization from game
				PreferDepthRangeZeroToOne         = true,
				PreferStandardClipSpaceYDirection = true,
			},
			graphicsBackend
		);

		_factory = _device.ResourceFactory;
	}

	public void Resize(Vector2Int size)
	{
		_device.ResizeMainWindow((uint)size.x, (uint)size.y);
	}

	public void Dispose()
	{
		_device.Dispose();
	}
	
	public void EndOfFrame()
	{
		_device.WaitForIdle();
		_device.SwapBuffers();
	}
}