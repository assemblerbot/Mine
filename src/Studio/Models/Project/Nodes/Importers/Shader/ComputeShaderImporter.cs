namespace Mine.Studio;

[Importer(ProjectNodeType.AssetComputeShader)]
public sealed class ComputeShaderImporter : ShaderImporter
{
	public ComputeShaderImporter(ProjectNode owner) : base(owner)
	{
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ShaderImporterSettings{ShaderStage = ShaderImporterStage.compute};
	}
}