namespace Mine.Studio;

[Importer(ProjectNodeType.AssetTessEvalShader)]
public sealed class ImporterTessEvalShader : ImporterShader
{
	public ImporterTessEvalShader(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ImporterShaderSettings{ShaderStage = ImporterShaderStage.tesseval};
	}
}