using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("34754bcb-aaff-4158-b85e-f6b2e2fb25db")]
public sealed class DefinitionTemplateFieldAssetReference : DefinitionTemplateField
{
	public override string TypeName => "AssetRef";
	public          string AssetType; // ???

	public DefinitionTemplateFieldAssetReference() : base("new")
	{
	}

	public DefinitionTemplateFieldAssetReference(string name) : base(name)
	{
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionTemplateFieldAssetReference))]
public interface IDefinitionTemplateFieldAssetReferenceMigratable : IDefinitionTemplateFieldMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionTemplateFieldAssetReference))]
public class DefinitionTemplateFieldAssetReference_000 : DefinitionTemplateField_000, IDefinitionTemplateFieldAssetReferenceMigratable
{
	public string AssetType;
}
#endregion