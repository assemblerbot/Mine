using Migration;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable, SerializedClassId("1d1cea4c-4339-4d76-a548-602210666e09")]
public sealed class StudioAssetDefinitionReference : StudioReference
{
	public override string Name => $"Definition<{GenericParameterGuid}>";
	public          string GenericParameterGuid;

	public StudioAssetDefinitionReference(string genericParameterGuid)
	{
		GenericParameterGuid = genericParameterGuid;
	}

	public override bool CanAcceptNode(StudioModel studioModel, ProjectNode node)
	{
		return node.Type == ProjectNodeType.AssetDefinition && node.GetNodeIO<NodeIOAssetDefinition>()?.Asset?.Template.Header.Guid == GenericParameterGuid;
	}

	public override StudioReference CreateCopy()
	{
		return new StudioAssetDefinitionReference(GenericParameterGuid) {Guid = Guid};
	}
}

#region Migration
[MigratableInterface(typeof(StudioAssetDefinitionReference))]
public interface IStudioAssetDefinitionReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioAssetDefinitionReference))]
public class StudioAssetDefinitionReference_000 : StudioReference_000, IStudioAssetDefinitionReferenceMigratable
{
	public string GenericParameterGuid;
}
#endregion