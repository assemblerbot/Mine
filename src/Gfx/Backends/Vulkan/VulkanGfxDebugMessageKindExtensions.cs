using Silk.NET.Vulkan;

namespace Gfx;

public static class VulkanGfxDebugMessageKindExtensions
{
	public static GfxDebugMessageKind ToGfxDebugMessageKind(this DebugUtilsMessageTypeFlagsEXT type)
	{
		if (type.HasFlag(DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt))
		{
			return GfxDebugMessageKind.Performance;
		}

		if (type.HasFlag(DebugUtilsMessageTypeFlagsEXT.ValidationBitExt))
		{
			return GfxDebugMessageKind.Validation;
		}

		if (type.HasFlag(DebugUtilsMessageTypeFlagsEXT.DeviceAddressBindingBitExt))
		{
			return GfxDebugMessageKind.Binding;
		}

		return GfxDebugMessageKind.General;
	}
}