using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("4018303e-635a-4a9d-9aa0-368586747f10")]
public sealed class DefinitionAssetValueAssetReference : DefinitionAssetValueReference
{
	public override string Name => "Asset";
}

#region Migration
[MigratableInterface(typeof(DefinitionAssetValueAssetReference))]
public interface IDefinitionAssetValueAssetReferenceMigratable : IDefinitionAssetValueReferenceMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueAssetReference))]
public class DefinitionAssetValueAssetReference_000 : DefinitionAssetValueReference_000, IDefinitionAssetValueAssetReferenceMigratable
{
}
#endregion