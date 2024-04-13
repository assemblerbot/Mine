using System.Runtime.InteropServices;
using Silk.NET.Windowing;

namespace Gfx;

public abstract class GfxApi : IDisposable
{
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

	public static GfxApi Create(GfxApiOptions options)
	{
		return options.Backend switch
		{
			GraphicsBackend.Vulkan => new VulkanGfxApi(options.Window, options.DebugMessageLog),
			_ => throw new ArgumentOutOfRangeException(nameof(options.Backend), options.Backend, null)
		};
	}

	public abstract void Dispose();

	public abstract IReadOnlyList<PhysicalDevice> EnumeratePhysicalDevices();
	
	public abstract GraphicsDevice CreateGraphicsDevice(IView window, GraphicsDeviceOptions options);
}