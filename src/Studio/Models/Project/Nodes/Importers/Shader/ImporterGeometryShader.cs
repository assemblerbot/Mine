namespace Mine.Studio;

[Importer(ProjectNodeType.AssetGeometryShader)]
public sealed class ImporterGeometryShader : ImporterShader
{
	public ImporterGeometryShader(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ImporterShaderSettings{ShaderStage = ImporterShaderStage.geometry};
	}
}