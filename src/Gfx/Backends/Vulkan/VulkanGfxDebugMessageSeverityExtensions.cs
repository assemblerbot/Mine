using Silk.NET.Vulkan;

namespace Gfx;

public static class VulkanGfxDebugMessageSeverityExtensions
{
	public static DebugMessageSeverity ToGfxDebugMessageSeverity(this DebugUtilsMessageSeverityFlagsEXT severity)
	{
		if (severity.HasFlag(DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt))
		{
			return DebugMessageSeverity.Error;
		}

		if (severity.HasFlag(DebugUtilsMessageSeverityFlagsEXT.WarningBitExt))
		{
			return DebugMessageSeverity.Warning;
		}

		return DebugMessageSeverity.Info;
	}
}