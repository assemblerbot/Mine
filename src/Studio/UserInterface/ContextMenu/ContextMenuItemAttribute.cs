namespace RedHerring.Studio.UserInterface;

[AttributeUsage(AttributeTargets.Class)]
public class ContextMenuItemAttribute : Attribute
{
	public readonly string MenuPath;

	public ContextMenuItemAttribute(string menuPath)
	{
		MenuPath = menuPath;
	}
}