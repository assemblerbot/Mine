namespace Mine.Studio;

[AttributeUsage(AttributeTargets.Field)]
public class ValueDropdownAttribute : Attribute
{
	public readonly string ItemsSource;

	public ValueDropdownAttribute(string itemsSource)
	{
		ItemsSource = itemsSource;
	}
}