using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Windowing;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Gfx;

//using GfxPhysicalDevice = Gfx.PhysicalDevice;

public sealed unsafe class VulkanGfxApi : GfxApi
{
	private static readonly string[] _validationLayers = 
	{
		"VK_LAYER_KHRONOS_validation",
	};

	private readonly IWindow  _window;
	private readonly Vk       _vk;
	private          Instance _instance;

	private ExtDebugUtils?                                          _debugUtils;
	private DebugUtilsMessengerEXT                                  _debugMessenger;
	private Action<DebugMessageSeverity, DebugMessageKind, string>? _debugMessageLog;
	private bool                                                    IsDebugEnabled => _debugMessageLog != null;

	private KhrSurface? _khrSurface;
	private SurfaceKHR  _surface;
	
	#region Lifecycle
	internal VulkanGfxApi(
		IWindow                                                 window,
		Action<DebugMessageSeverity, DebugMessageKind, string>? debugMessageLog
	)
	{
		_window          = window;
		_debugMessageLog = debugMessageLog;
		_vk              = Vk.GetApi();

		CreateInstance();
		SetupDebugMessenger();
		CreateSurface();

		_vk.GetPhysicalDeviceProperties(new Silk.NET.Vulkan.PhysicalDevice(), out PhysicalDeviceProperties properties);
	}

	public override void Dispose()
	{
		if (IsDebugEnabled)
		{
			_debugUtils!.DestroyDebugUtilsMessenger(_instance, _debugMessenger, null);
		}

		_khrSurface?.DestroySurface(_instance, _surface, null);
		_vk.DestroyInstance(_instance, null);
		_vk.Dispose();
	}
	#endregion Lifecycle

	#region Base overrides
	public override IReadOnlyList<PhysicalDevice> EnumeratePhysicalDevices()
	{
		IReadOnlyCollection<Silk.NET.Vulkan.PhysicalDevice>? devices = _vk.GetPhysicalDevices(_instance);
		return devices.Where(IsDeviceSuitable).Select(device => new VulkanPhysicalDevice()).ToList();
	}

	public override GraphicsDevice CreateGraphicsDevice(IView window, GraphicsDeviceOptions options)
	{
		// TODO
		return new VulkanGraphicsDevice(options);
	}
	
	#endregion Base overrides
	
	#region Private
	private void CreateInstance()
	{
		if (IsDebugEnabled && !ValidationLayersSupported())
		{
			throw new GfxException("Validation layers are not supported!");
		}

		ApplicationInfo appInfo = new()
		                          {
			                          SType              = StructureType.ApplicationInfo,
			                          PApplicationName   = (byte*)Marshal.StringToHGlobalAnsi(_window.Title),
			                          ApplicationVersion = new Version32(1, 0, 0),
			                          PEngineName        = (byte*)Marshal.StringToHGlobalAnsi("Mine"),
			                          EngineVersion      = new Version32(1,                                      0,                                      0),
			                          ApiVersion         = new Version32((uint)_window.API.Version.MajorVersion, (uint)_window.API.Version.MinorVersion, 0U)
		                          };

		InstanceCreateInfo createInfo = new()
		                                {
			                                SType            = StructureType.InstanceCreateInfo,
			                                PApplicationInfo = &appInfo,
			                                Flags = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
				                                ? InstanceCreateFlags.EnumeratePortabilityBitKhr
				                                : InstanceCreateFlags.None,
		                                };

		string[] extensions = GetRequiredExtensions();
		createInfo.EnabledExtensionCount   = (uint)extensions.Length;
		createInfo.PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(extensions); ;
		
		if (IsDebugEnabled)
		{
			createInfo.EnabledLayerCount   = (uint)_validationLayers.Length;
			createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(_validationLayers);

			DebugUtilsMessengerCreateInfoEXT debugCreateInfo = new();
			PopulateDebugMessengerCreateInfo(ref debugCreateInfo);
			createInfo.PNext = &debugCreateInfo;
		}
		else
		{
			createInfo.EnabledLayerCount = 0;
			createInfo.PNext             = null;
		}

		if (_vk.CreateInstance(createInfo, null, out _instance) != Result.Success)
		{
			throw new GfxException("Failed to create Vulkan instance!");
		}

		Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
		Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);
		SilkMarshal.Free((nint)createInfo.PpEnabledExtensionNames);

		if (IsDebugEnabled)
		{
			SilkMarshal.Free((nint)createInfo.PpEnabledLayerNames);
		}
	}

	private void SetupDebugMessenger()
	{
		if (!IsDebugEnabled)
		{
			return;
		}

		if (!_vk.TryGetInstanceExtension(_instance, out _debugUtils))
		{
			return;
		}

		DebugUtilsMessengerCreateInfoEXT createInfo = new();
		PopulateDebugMessengerCreateInfo(ref createInfo);

		if (_debugUtils!.CreateDebugUtilsMessenger(_instance, in createInfo, null, out _debugMessenger) != Result.Success)
		{
			throw new GfxException("Failed to set up debug messenger!");
		}
	}

	private void CreateSurface()
	{
		if (!_vk.TryGetInstanceExtension<KhrSurface>(_instance, out _khrSurface))
		{
			throw new GfxException("KHR_surface extension not found.");
		}

		_surface = _window.VkSurface!.Create<AllocationCallbacks>(_instance.ToHandle(), null).ToSurface();
	}

	private bool ValidationLayersSupported()
	{
		uint layerCount = 0;
		_vk.EnumerateInstanceLayerProperties(ref layerCount, null);
		var availableLayers = new LayerProperties[layerCount];
		fixed (LayerProperties* availableLayersPtr = availableLayers)
		{
			_vk.EnumerateInstanceLayerProperties(ref layerCount, availableLayersPtr);
		}

		HashSet<string?> availableLayerNames = availableLayers.Select(layer => { return Marshal.PtrToStringAnsi((IntPtr) layer.LayerName); }).ToHashSet();

		return _validationLayers.All(availableLayerNames.Contains);
	}

	private string[] GetRequiredExtensions()
	{
		byte**    windowExtensions = _window!.VkSurface!.GetRequiredExtensions(out var windowExtensionCount);
		string[]? extensions       = SilkMarshal.PtrToStringArray((nint)windowExtensions, (int)windowExtensionCount);

		if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			extensions = extensions.Append("VK_KHR_portability_enumeration").ToArray();
		}

		if (IsDebugEnabled)
		{
			extensions = extensions.Append(ExtDebugUtils.ExtensionName).ToArray();
		}
		
		return extensions;
	}
	
	private void PopulateDebugMessengerCreateInfo(ref DebugUtilsMessengerCreateInfoEXT createInfo)
	{
		createInfo.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
		createInfo.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
		                             DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
		                             DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt;
		createInfo.MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt     |
		                         DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
		                         DebugUtilsMessageTypeFlagsEXT.ValidationBitExt;
		createInfo.PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)DebugCallback;
	}
	
	private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
	{
		_debugMessageLog!.Invoke(messageSeverity.ToGfxDebugMessageSeverity(), messageTypes.ToGfxDebugMessageKind(), Marshal.PtrToStringAnsi((nint) pCallbackData->PMessage) ?? ""); 
		return Vk.False;
	}

	private bool IsDeviceSuitable(Silk.NET.Vulkan.PhysicalDevice physicalDevice)
	{
		//VulkanQueueFamilyIndices indices = FindQueueFamilyIndices(physicalDevice);
		return true;
	}

	// private VulkanQueueFamilyIndices FindQueueFamilyIndices(Silk.NET.Vulkan.PhysicalDevice physicalDevice)
	// {
	// 	
	// }

	#endregion
}