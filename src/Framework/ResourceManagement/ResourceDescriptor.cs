namespace GameToolkit.Framework;

public readonly struct ResourceDescriptor
{
	public readonly ResourceSourceType ResourceSourceType;
	public readonly string             SourceFilePath;
	public readonly ResourceType	   ResourceType;

	public ResourceDescriptor(ResourceSourceType resourceSourceType, string sourceFilePath)
	{
		ResourceSourceType = resourceSourceType;
		SourceFilePath     = sourceFilePath;
		ResourceType       = ResourceType.Unknown;
	}
}