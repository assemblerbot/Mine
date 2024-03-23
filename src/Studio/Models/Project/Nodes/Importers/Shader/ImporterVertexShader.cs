namespace Mine.Studio;

[Importer(ProjectNodeType.AssetVertexShader)]
public sealed class ImporterVertexShader : ImporterShader
{
	public ImporterVertexShader(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ImporterShaderSettings{ShaderStage = ImporterShaderStage.vertex};
	}
}