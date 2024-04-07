using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Gfx;

/*

 https://veldrid.dev/articles/getting-started/getting-started-part2.html

 */

namespace Mine.Framework;

public class Graphics : IDisposable
{
	private GfxApi          _api;
	private GraphicsDevice  _device;

	public GfxApi         Api    => _api;
	public GraphicsDevice Device => _device;

	public Graphics(IView view, GraphicsBackend graphicsBackend)
	{
		_api = GfxApi.Create(graphicsBackend);
		
		_device = _api.CreateGraphicsDevice(
			view,
			new GraphicsDeviceOptions
			{
				// TODO - customization from game
				//PreferDepthRangeZeroToOne         = true,
				//PreferStandardClipSpaceYDirection = true,
				//SwapchainDepthFormat = PixelFormat.D32_Float_S8_UInt, // PixelFormat.D24_UNorm_S8_UInt
			}
		);
	}

	public void Resize(Vector2Int size)
	{
		_device.ResizeMainWindow((uint)size.X, (uint)size.Y);
	}

	public void Dispose()
	{
		_device.Dispose();
	}

	public void BeginOfFrame()
	{
	}

	public void EndOfFrame()
	{
		_device.WaitForIdle();
		_device.SwapBuffers();
	}
}