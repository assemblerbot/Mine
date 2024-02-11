namespace Mine.Studio;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ReadOnlyInInspectorAttribute : Attribute
{
}