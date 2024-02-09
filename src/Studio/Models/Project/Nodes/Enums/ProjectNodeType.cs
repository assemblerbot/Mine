namespace RedHerring.Studio.Models.Project.FileSystem;

public enum ProjectNodeType
{
	Uninitialized,
	
	AssetFolder,
	AssetImage,
	AssetScene,
	AssetBinary,
	AssetMaterial,
	
	ScriptFolder,
	ScriptFile,
}

public static class ProjectNodeTypeExtensions
{
	public static bool IsAssetsRelated(this ProjectNodeType type)
	{
		return
			type == ProjectNodeType.AssetFolder     ||
			type == ProjectNodeType.AssetImage      ||
			type == ProjectNodeType.AssetScene      ||
			type == ProjectNodeType.AssetBinary     ||
			type == ProjectNodeType.AssetMaterial
			;
	}

	public static bool IsScriptsRelated(this ProjectNodeType type)
	{
		return
			type == ProjectNodeType.ScriptFolder ||
			type == ProjectNodeType.ScriptFile
			;
	}

	public static ProjectNodeType FromAssetExtension(string extension)
	{
		return extension switch
		{
			".png" => ProjectNodeType.AssetImage,
			".jpg" => ProjectNodeType.AssetImage,
			".fbx" => ProjectNodeType.AssetScene,
			".obj" => ProjectNodeType.AssetScene,
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