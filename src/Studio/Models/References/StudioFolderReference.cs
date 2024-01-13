using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("293da6d5-96f3-4057-9905-b20cffe6316a")]
public sealed class StudioFolderReference : StudioReference
{
	public override string Name => "Folder";
}

#region Migration
[MigratableInterface(typeof(StudioFolderReference))]
public interface IStudioFolderReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioFolderReference))]
public class StudioFolderReference_000 : StudioReference_000, IStudioFolderReferenceMigratable
{
}
#endregion