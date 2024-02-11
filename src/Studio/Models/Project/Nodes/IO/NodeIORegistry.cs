using Mine.Framework;

namespace Mine.Studio;

public sealed class NodeIORegistry
{
	private readonly Dictionary<ProjectNodeType, Type> _types    = new();
	private readonly Type                              _fallback = typeof(NodeIOCopy);

	public NodeIORegistry()
	{
		ScanTypes();
	}

	public NodeIO CreateNodeIO(ProjectNode node)
	{
		Type   nodeIOType = _types.TryGetValue(node.Type, out Type? type) ? type : _fallback;
		object instance   = Activator.CreateInstance(nodeIOType, node)!;
		return (NodeIO) instance;
	}
	
	private void ScanTypes()
	{
		Engine.Types.ForEachAttribute<NodeIOAttribute>(
			(attribute, type) =>
			{
				_types.Add(attribute.NodeType, type);
			}
		);
	}
}