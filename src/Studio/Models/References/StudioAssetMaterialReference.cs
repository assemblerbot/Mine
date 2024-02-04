using Migration;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable, SerializedClassId("1055c024-4148-4895-ab0d-788bc0233338")]
public sealed class StudioAssetMaterialReference : StudioReference
{
	public override string Name => "Material";

	public override bool CanAcceptNode(StudioModel studioModel, ProjectNode node)
	{
		return node.Type == ProjectNodeType.AssetMaterial;
	}

	public override StudioReference CreateCopy()
	{
		return new StudioAssetMaterialReference {Guid = Guid};
	}
}

#region Migration
[MigratableInterface(typeof(StudioAssetMaterialReference))]
public interface IStudioAssetMaterialReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioAssetMaterialReference))]
public class StudioAssetMaterialReference_000 : StudioReference_000, IStudioAssetMaterialReferenceMigratable
{
}
#endregion