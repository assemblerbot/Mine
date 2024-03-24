namespace Mine.Studio;

[Importer(ProjectNodeType.AssetPixelShader)]
public sealed class PixelShaderImporter : ShaderImporter
{
	public PixelShaderImporter(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ShaderImporterSettings{ShaderStage = ShaderImporterStage.fragment};
	}
}