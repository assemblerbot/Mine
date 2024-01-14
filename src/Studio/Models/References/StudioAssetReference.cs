using Migration;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable, SerializedClassId("4018303e-635a-4a9d-9aa0-368586747f10")]
public sealed class StudioAssetReference : StudioReference
{
	public override string Name => "Asset";
	public override bool   CanAcceptNode(ProjectNode node)
	{
		return node.Type.IsAssetsRelated() && node.Type != ProjectNodeType.AssetFolder;
	}

	public override StudioReference CreateCopy()
	{
		return new StudioAssetReference {Guid = Guid};
	}
}

#region Migration
[MigratableInterface(typeof(StudioAssetReference))]
public interface IStudioAssetReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioAssetReference))]
public class StudioAssetReference_000 : StudioReference_000, IStudioAssetReferenceMigratable
{
}
#endregion