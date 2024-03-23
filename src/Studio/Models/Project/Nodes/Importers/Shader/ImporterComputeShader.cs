namespace Mine.Studio;

[Importer(ProjectNodeType.AssetComputeShader)]
public sealed class ImporterComputeShader : ImporterShader
{
	public ImporterComputeShader(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ImporterShaderSettings{ShaderStage = ImporterShaderStage.compute};
	}
}