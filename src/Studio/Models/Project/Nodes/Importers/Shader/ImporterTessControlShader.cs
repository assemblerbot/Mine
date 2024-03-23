namespace Mine.Studio;

[Importer(ProjectNodeType.AssetTessControlShader)]
public sealed class ImporterTessControlShader : ImporterShader
{
	public ImporterTessControlShader(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ImporterShaderSettings{ShaderStage = ImporterShaderStage.tesscontrol};
	}
}