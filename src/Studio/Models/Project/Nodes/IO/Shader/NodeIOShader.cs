using Mine.Framework;

namespace Mine.Studio;

public abstract class NodeIOShader : NodeIO
{
	private static readonly string _compilerAbsolutePath = Path.Join(Engine.Paths.ApplicationAbsolutePath, "Tools\\win-x64\\glslc.exe"); // TODO - platform
	public override         string ReferenceType => nameof(AssetReference);                                                             // TODO - remove ?

	public NodeIOShader(ProjectNode owner) : base(owner)
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

		NodeIOShaderSettings? settings = Owner.Meta?.NodeIOSettings as NodeIOShaderSettings;
		if (settings is null)
		{
			ConsoleViewModel.LogError($"Cannot import '{Owner.RelativePath}' - settings are missing or invalid!");
			relativeResourcePath = null;
			return;
		}

		string arguments = $"-fentry-point={settings.EntryPoint} -fshader-stage={settings.ShaderStage} -o \"{targetPath}\" \"{Owner.AbsolutePath}\"";
		ConsoleViewModel.LogInfo($"Executing: {_compilerAbsolutePath} {arguments}");
		string outputLog = RunUtility.RunExternalExe(_compilerAbsolutePath, arguments);
		ConsoleViewModel.LogInfo(outputLog);
	}

	public override bool UpdateImportSettings(NodeIOSettings settings)
	{
		return false;
	}
}
