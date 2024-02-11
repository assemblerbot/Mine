namespace Mine.Studio;

[AttributeUsage(AttributeTargets.Class)]
public sealed class NodeIOAttribute : Attribute
{
	public readonly ProjectNodeType NodeType; 
	
	public NodeIOAttribute(ProjectNodeType nodeType = ProjectNodeType.Uninitialized)
	{
		NodeType   = nodeType;
	}
}