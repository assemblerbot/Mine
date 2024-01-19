using Migration;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable, SerializedClassId("1d1cea4c-4339-4d76-a548-602210666e09")]
public sealed class StudioAssetDefinitionReference : StudioReference
{
	public override string Name => $"Definition<{GenericParameter}>";
	public          string GenericParameter;

	public StudioAssetDefinitionReference(string genericParameter)
	{
		GenericParameter = genericParameter;
	}

	public override bool CanAcceptNode(ProjectNode node)
	{
		return node.Type == ProjectNodeType.AssetDefinition; // TODO - also type of definition
	}

	public override StudioReference CreateCopy()
	{
		return new StudioAssetDefinitionReference(GenericParameter) {Guid = Guid};
	}
}

#region Migration
[MigratableInterface(typeof(StudioAssetDefinitionReference))]
public interface IStudioAssetDefinitionReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioAssetDefinitionReference))]
public class StudioAssetDefinitionReference_000 : StudioReference_000, IStudioAssetDefinitionReferenceMigratable
{
	public string GenericParameter;
}
#endregion