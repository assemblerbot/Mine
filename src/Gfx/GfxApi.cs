using System.Runtime.InteropServices;
using Silk.NET.Windowing;

namespace Gfx;

/// <summary>
/// A communication API with underlying native graphics library.
/// </summary>
public abstract class GfxApi : IDisposable
{
	/// <summary>
	/// Gets the default backend given the current runtime information.
	/// </summary>
	/// <returns>The default backend for this runtime/platform.</returns>
	public static GraphicsBackend GetDefaultBackend()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return GraphicsBackend.Vulkan;
		}
		
		if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return GraphicsBackend.Vulkan;
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return GraphicsBackend.Vulkan;
		}

		return GraphicsBackend.Vulkan;
	}

	/// <summary>
	/// Creates a new instance of <see cref="GfxApi"/>.
	/// </summary>
	/// <param name="backend">The desired graphics backend.</param>
	/// <returns>A new <see cref="GfxApi"/>.</returns>
	public static GfxApi Create(GraphicsBackend backend, IWindow window)
	{
		return backend switch
		{
			GraphicsBackend.Vulkan => new VulkanGfxApi(window),
			_ => throw new ArgumentOutOfRangeException(nameof(backend), backend, null)
		};
	}

	/// <summary>
	/// Dispose instance of <see cref="GfxApi"/>
	/// </summary>
	public abstract void Dispose();
	
	/// <summary>
	/// Creates a new instance of <see cref="GraphicsDevice"/>.
	/// </summary>
	/// <param name="options">The desired properties of the created object.</param>
	/// <returns>A new <see cref="GraphicsDevice"/>.</returns>
	public abstract GraphicsDevice CreateGraphicsDevice(IView window, GraphicsDeviceOptions options);
}