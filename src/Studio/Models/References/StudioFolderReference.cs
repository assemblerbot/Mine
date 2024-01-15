using Migration;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable, SerializedClassId("293da6d5-96f3-4057-9905-b20cffe6316a")]
public sealed class StudioFolderReference : StudioReference
{
	public override string Name => "Folder";
	
	public override bool   CanAcceptNode(ProjectNode node)
	{
		return node.Type == ProjectNodeType.AssetFolder;
	}

	public override StudioReference CreateCopy()
	{
		return new StudioFolderReference {Guid = Guid};
	}
}

#region Migration
[MigratableInterface(typeof(StudioFolderReference))]
public interface IStudioFolderReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioFolderReference))]
public class StudioFolderReference_000 : StudioReference_000, IStudioFolderReferenceMigratable
{
}
#endregion