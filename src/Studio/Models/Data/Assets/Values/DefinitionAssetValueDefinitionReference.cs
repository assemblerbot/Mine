using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("1d1cea4c-4339-4d76-a548-602210666e09")]
public sealed class DefinitionAssetValueDefinitionReference : DefinitionAssetValueReference
{
	public override string Name => $"Definition<{GenericParameter}>";
	public          string GenericParameter;

	public DefinitionAssetValueDefinitionReference(string genericParameter)
	{
		GenericParameter = genericParameter;
	}
}

#region Migration
[MigratableInterface(typeof(DefinitionAssetValueDefinitionReference))]
public interface IDefinitionAssetValueDefinitionReferenceMigratable : IDefinitionAssetValueReferenceMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueDefinitionReference))]
public class DefinitionAssetValueDefinitionReference_000 : DefinitionAssetValueReference_000, IDefinitionAssetValueDefinitionReferenceMigratable
{
	public string GenericParameter;
}
#endregion