using Mine.Framework;

namespace Mine.Studio;

public sealed class ImporterRegistry
{
	private readonly Dictionary<ProjectNodeKind, Type> _types    = new();
	private readonly Type                              _fallback = typeof(CopyImporter);

	public ImporterRegistry()
	{
		ScanTypes();
	}

	public Importer CreateImporter(ProjectNode node)
	{
		Type   importerType = _types.TryGetValue(node.Kind, out Type? type) ? type : _fallback;
		object instance   = Activator.CreateInstance(importerType, node)!;
		return (Importer) instance;
	}
	
	private void ScanTypes()
	{
		Engine.Types.ForEachAttribute<ImporterAttribute>(
			(attribute, type) =>
			{
				_types.Add(attribute.NodeKind, type);
			}
		);
	}
}