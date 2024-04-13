using Silk.NET.Vulkan;

namespace Gfx;

public static class VulkanGfxDebugMessageKindExtensions
{
	public static DebugMessageKind ToGfxDebugMessageKind(this DebugUtilsMessageTypeFlagsEXT type)
	{
		if (type.HasFlag(DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt))
		{
			return DebugMessageKind.Performance;
		}

		if (type.HasFlag(DebugUtilsMessageTypeFlagsEXT.ValidationBitExt))
		{
			return DebugMessageKind.Validation;
		}

		if (type.HasFlag(DebugUtilsMessageTypeFlagsEXT.DeviceAddressBindingBitExt))
		{
			return DebugMessageKind.Binding;
		}

		return DebugMessageKind.General;
	}
}