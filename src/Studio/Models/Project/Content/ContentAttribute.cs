namespace Mine.Studio;

[AttributeUsage(AttributeTargets.Class)]
public class ContentAttribute : System.Attribute
{
	public readonly string[] Extensions;
	
	public ContentAttribute(params string[] extensions)
	{
		Extensions = extensions;
	}
}