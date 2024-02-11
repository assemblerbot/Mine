namespace Mine.Studio;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ToolAttribute : Attribute
{
	public readonly string Name;

	public ToolAttribute(string name)
	{
		Name = name;
	}
}