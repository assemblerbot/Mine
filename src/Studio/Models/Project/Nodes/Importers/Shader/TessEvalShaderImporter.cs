namespace Mine.Studio;

[Importer(ProjectNodeKind.AssetTessEvalShader)]
public sealed class TessEvalShaderImporter : ShaderImporter
{
	public TessEvalShaderImporter(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ShaderImporterSettings{ShaderStage = ShaderImporterStage.tesseval};
	}
}