using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;

namespace Mine.Studio;

[NodeIO(ProjectNodeType.ScriptDefinition)]
public sealed class NodeIOScriptDefinition : NodeIO
{
	private DefinitionTemplate? _template = null;
	public  DefinitionTemplate? Template => _template;
	
	public NodeIOScriptDefinition(ProjectNode owner) : base(owner)
	{
	}

	public override void Update()
	{
		
	}

	public override void Load()
	{
		if (_template is not null)
		{
			return;
		}

		_template = DefinitionTemplate.CreateFromFile(Owner.AbsolutePath, Owner.Project);
	}

	public override void Save()
	{
		if (_template is null)
		{
			return;
		}

		_template.WriteToFile(Owner.AbsolutePath, Owner.Meta!.Guid, Owner.Project);
	}

	public override void ClearCache()
	{
		_template = null;
	}

	public override void Import(string resourcePath)
	{
		throw new InvalidOperationException();
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOSettings();
	}
}