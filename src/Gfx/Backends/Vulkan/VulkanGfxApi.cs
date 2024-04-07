using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Windowing;
using Silk.NET.Vulkan;

namespace Gfx;

public sealed unsafe class VulkanGfxApi : GfxApi
{
	private readonly Vk       _vk;
	private          Instance _instance;
	
	public VulkanGfxApi(IWindow window)
	{
		_vk = Vk.GetApi();
		
		ApplicationInfo appInfo = new()
		                          {
			                          SType              = StructureType.ApplicationInfo,
			                          PApplicationName   = (byte*)Marshal.StringToHGlobalAnsi(window.Title),
			                          ApplicationVersion = new Version32(1, 0, 0),
			                          PEngineName        = (byte*)Marshal.StringToHGlobalAnsi("Mine"),
			                          EngineVersion      = new Version32(1,                                     0,                                     0),
			                          ApiVersion         = new Version32((uint)window.API.Version.MajorVersion, (uint)window.API.Version.MinorVersion, 0U)
		                          };

		InstanceCreateInfo createInfo = new()
		                                {
			                                SType            = StructureType.InstanceCreateInfo,
			                                PApplicationInfo = &appInfo,
			                                Flags            = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? InstanceCreateFlags.EnumeratePortabilityBitKhr : InstanceCreateFlags.None,
		                                };
        
		byte** vkExtensions = window.VkSurface!.GetRequiredExtensions(out uint vkExtensionCount);
        
		if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			string[]? extensions = SilkMarshal.PtrToStringArray((nint)vkExtensions, (int)vkExtensionCount);
			extensions       = extensions.Append("VK_KHR_portability_enumeration").ToArray();
			vkExtensionCount = (uint) extensions.Length;
			vkExtensions     = (byte**)SilkMarshal.StringArrayToPtr(extensions);
		}

		createInfo.EnabledExtensionCount   = vkExtensionCount;
		createInfo.PpEnabledExtensionNames = vkExtensions;
		createInfo.EnabledLayerCount       = 0;

		if (_vk.CreateInstance(createInfo, null, out _instance) != Result.Success)
		{
			throw new GfxException("Failed to create Vulkan instance!");
		}

		Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
		Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);
	}

	public override void Dispose()
	{
		_vk.DestroyInstance(_instance, null);
		_vk.Dispose();
	}

	public override GraphicsDevice CreateGraphicsDevice(IView window, GraphicsDeviceOptions options)
	{
		return new VulkanGraphicsDevice(options);
	}
}