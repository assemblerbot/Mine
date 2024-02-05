using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;

namespace Mine.Studio;

[NodeIO(ProjectNodeType.ScriptDefinition)]
public sealed class NodeIOScriptDefinition : NodeIO<DefinitionTemplate>
{
	private DefinitionTemplate? _template = null;
	public  DefinitionTemplate? Template => _template;
	
	public NodeIOScriptDefinition(ProjectNode owner) : base(owner)
	{
	}

	public override void UpdateCache()
	{
		_template = DefinitionTemplate.CreateFromFile(Owner.AbsolutePath, Owner.Project, true);
	}

	public override DefinitionTemplate? Load()
	{
		return DefinitionTemplate.CreateFromFile(Owner.AbsolutePath, Owner.Project, false);
	}

	public override void Save(DefinitionTemplate data)
	{
		data.WriteToFile(Owner.AbsolutePath, Owner.Meta!.Guid, Owner.Project);
	}

	public override void ClearCache()
	{
		_template = null;
	}

	public override string? Import(string resourcesRootPath)
	{
		throw new InvalidOperationException();
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOSettings();
	}

	public override bool UpdateImportSettings(NodeIOSettings settings)
	{
		return false;
	}
}