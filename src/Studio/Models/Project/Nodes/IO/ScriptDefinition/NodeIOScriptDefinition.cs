using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[NodeIO(ProjectNodeType.ScriptDefinition)]
public sealed class NodeIOScriptDefinition : NodeIO
{
	public NodeIOScriptDefinition(ProjectNode owner) : base(owner)
	{
	}

	public override void Load()
	{
		throw new NotImplementedException();
	}

	public override void Save()
	{
		throw new NotImplementedException();
	}

	public override void Import(string resourcePath)
	{
		throw new NotImplementedException();
	}

	public override void ClearCache()
	{
		throw new NotImplementedException();
	}
}