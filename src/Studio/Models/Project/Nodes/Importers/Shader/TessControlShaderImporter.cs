namespace Mine.Studio;

[Importer(ProjectNodeType.AssetTessControlShader)]
public sealed class TessControlShaderImporter : ShaderImporter
{
	public TessControlShaderImporter(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ShaderImporterSettings{ShaderStage = ShaderImporterStage.tesscontrol};
	}
}