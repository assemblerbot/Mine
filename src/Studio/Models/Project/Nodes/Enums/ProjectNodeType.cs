namespace Mine.Studio;

public enum ProjectNodeType
{
	Uninitialized = 0,
	
	AssetFlag     = 0x01,
	AssetFolder   = 1 << 8 | AssetFlag,
	AssetImage    = 2 << 8 | AssetFlag,
	AssetScene    = 3 << 8 | AssetFlag,
	AssetBinary   = 4 << 8 | AssetFlag,
	AssetMaterial = 5 << 8 | AssetFlag,
	AssetShader   = 6 << 8 | AssetFlag,
	
	ScriptFlag   = 0x02,
	ScriptFolder = 1 << 8 | ScriptFlag,
	ScriptFile   = 2 << 8 | ScriptFlag,
	
	FlagMask = 0xff
}

public static class ProjectNodeTypeExtensions
{
	public static bool IsAssetsRelated(this ProjectNodeType type)
	{
		return ((uint) type & (uint) ProjectNodeType.FlagMask) == (uint) ProjectNodeType.AssetFlag;
	}

	public static bool IsScriptsRelated(this ProjectNodeType type)
	{
		return ((uint) type & (uint) ProjectNodeType.FlagMask) == (uint) ProjectNodeType.ScriptFlag;
	}

	public static ProjectNodeType FromAssetExtension(string extension)
	{
		return extension switch
		{
			".png" => ProjectNodeType.AssetImage,
			".jpg" => ProjectNodeType.AssetImage,
			".fbx" => ProjectNodeType.AssetScene,
			".obj" => ProjectNodeType.AssetScene,
			".hlsl" => ProjectNodeType.AssetShader,
			".glsl" => ProjectNodeType.AssetShader,
			".material" => ProjectNodeType.AssetMaterial,
			_ => ProjectNodeType.AssetBinary
		};
	}

	public static ProjectNodeType FromScriptType(string scriptType)
	{
		return scriptType switch
		{
			//ScriptNodeTypes.DEFINITION => ProjectNodeType.ScriptDefinition,
			_ => ProjectNodeType.ScriptFile
		};
	}
}