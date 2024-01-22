using Migration;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable, SerializedClassId("b0ca24a7-dd43-44ab-ba21-2f8a0cf8a392")]
public class StudioScriptReference : StudioReference
{
	public override string Name => "Script";

	public override bool CanAcceptNode(StudioModel studioModel, ProjectNode node)
	{
		return node.Type.IsScriptsRelated() && node.Type != ProjectNodeType.ScriptFolder;
	}

	public override StudioReference CreateCopy()
	{
		return new StudioScriptReference {Guid = Guid};
	}
}

#region Migration
[MigratableInterface(typeof(StudioScriptReference))]
public interface IStudioScriptReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioScriptReference))]
public class StudioScriptReference_000 : StudioReference_000, IStudioScriptReferenceMigratable
{
}
#endregion