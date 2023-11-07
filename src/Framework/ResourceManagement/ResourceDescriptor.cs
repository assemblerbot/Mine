namespace GameToolkit.Framework;

public readonly struct ResourceDescriptor
{
	public readonly ResourceType ResourceType;
	public readonly string       FilePath;

	public ResourceDescriptor(ResourceType resourceType, string filePath)
	{
		ResourceType = resourceType;
		FilePath = filePath;
	}
}