namespace Gfx;

public struct VulkanQueueFamilyIndices
{
	public uint? GraphicsFamily;
	public uint? PresentFamily;

	public bool IsComplete()
	{
		return GraphicsFamily.HasValue && PresentFamily.HasValue;
	}
}