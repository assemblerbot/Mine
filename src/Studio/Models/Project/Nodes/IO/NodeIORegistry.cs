using Mine.Framework;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public sealed class NodeIORegistry
{
	private readonly Dictionary<ProjectNodeType, Type> _types    = new();
	private readonly Type                              _fallback = typeof(NodeIOCopy);

	public NodeIORegistry()
	{
		ScanTypes();
	}

	public NodeIO CreateNodeIO(ProjectNodeType projectNodeType)
	{
		Type   nodeIOType = _types.TryGetValue(projectNodeType, out Type? type) ? type : _fallback;
		object instance   = Activator.CreateInstance(nodeIOType)!;
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