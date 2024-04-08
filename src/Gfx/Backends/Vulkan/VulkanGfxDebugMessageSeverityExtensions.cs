using Silk.NET.Vulkan;

namespace Gfx;

public static class VulkanGfxDebugMessageSeverityExtensions
{
	public static GfxDebugMessageSeverity ToGfxDebugMessageSeverity(this DebugUtilsMessageSeverityFlagsEXT severity)
	{
		if (severity.HasFlag(DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt))
		{
			return GfxDebugMessageSeverity.Error;
		}

		if (severity.HasFlag(DebugUtilsMessageSeverityFlagsEXT.WarningBitExt))
		{
			return GfxDebugMessageSeverity.Warning;
		}

		return GfxDebugMessageSeverity.Info;
	}
}