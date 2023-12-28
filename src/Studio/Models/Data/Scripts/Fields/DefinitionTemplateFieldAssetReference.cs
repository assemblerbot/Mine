namespace Mine.Studio;

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