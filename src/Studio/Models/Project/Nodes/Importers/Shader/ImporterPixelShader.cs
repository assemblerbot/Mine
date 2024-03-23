namespace Mine.Studio;

[Importer(ProjectNodeType.AssetPixelShader)]
public sealed class ImporterPixelShader : ImporterShader
{
	public ImporterPixelShader(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ImporterShaderSettings{ShaderStage = ImporterShaderStage.fragment};
	}
}