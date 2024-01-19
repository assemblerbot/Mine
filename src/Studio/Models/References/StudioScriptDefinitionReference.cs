using Migration;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable, SerializedClassId("f251f5e1-fd17-4a98-937e-4849d5a56e5c")]
public class StudioScriptDefinitionReference : StudioReference
{
	public override string Name => "Definition template";

	public override bool CanAcceptNode(ProjectNode node)
	{
		return node.Type == ProjectNodeType.ScriptDefinition;
	}

	public override StudioReference CreateCopy()
	{
		return new StudioScriptDefinitionReference {Guid = Guid};
	}
}

#region Migration
[MigratableInterface(typeof(StudioScriptDefinitionReference))]
public interface IStudioScriptDefinitionReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioScriptDefinitionReference))]
public class StudioScriptDefinitionReference_000 : StudioReference_000, IStudioScriptDefinitionReferenceMigratable
{
}
#endregion