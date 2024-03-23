using Mine.Framework;

namespace Mine.Studio;

public abstract class ImporterShader : Importer
{
	private static readonly string _compilerAbsolutePath = Path.Join(Engine.Paths.ApplicationAbsolutePath, "Tools\\win-x64\\glslc.exe"); // TODO - platform
	public override         string ReferenceType => nameof(AssetReference);                                                             // TODO - remove ?

	public ImporterShader(ProjectNode owner) : base(owner)
	{
	}

	public override void UpdateCache()
	{
	}

	public override void ClearCache()
	{
	}

	public override void Import(string resourcesRootPath, out string? relativeResourcePath)
	{
		relativeResourcePath = Owner.RelativePath.Replace(".hlsl", ".spirv").Replace(".glsl", ".spirv");
		string targetPath         = Path.Join(resourcesRootPath, relativeResourcePath);

		ImporterShaderSettings? settings = Owner.Meta?.ImporterSettings as ImporterShaderSettings;
		if (settings is null)
		{
			ConsoleViewModel.LogError($"Cannot import '{Owner.RelativePath}' - settings are missing or invalid!");
			relativeResourcePath = null;
			return;
		}

		string arguments = $"-fentry-point={settings.EntryPoint} -fshader-stage={settings.ShaderStage} -o \"{targetPath}\" \"{Owner.AbsolutePath}\"";
		ConsoleViewModel.LogInfo($"Executing: {_compilerAbsolutePath} {arguments}");
		string outputLog = FileExecutionUtility.ExecuteFile(_compilerAbsolutePath, arguments);
		ConsoleViewModel.LogInfo(outputLog);
	}

	public override bool UpdateImportSettings(ImporterSettings settings)
	{
		return false;
	}
}
