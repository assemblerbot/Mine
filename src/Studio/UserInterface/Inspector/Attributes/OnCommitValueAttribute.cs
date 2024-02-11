namespace Mine.Studio;

[AttributeUsage(AttributeTargets.Field)]
public sealed class OnCommitValueAttribute : Attribute
{
	public readonly string MethodName;

	public OnCommitValueAttribute(string methodName)
	{
		MethodName = methodName;
	}
}