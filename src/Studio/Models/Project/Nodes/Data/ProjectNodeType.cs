namespace RedHerring.Studio.Models.Project.FileSystem;

public enum ProjectNodeType
{
	Uninitialized,
	
	AssetFolder,
	AssetImage,
	AssetMesh,
	AssetBinary,
	AssetDefinition,
	
	ScriptFolder,
	ScriptFile,
	ScriptDefinition,
}

public static class ProjectNodeTypeExtensions
{
	public static bool IsAssetsRelated(this ProjectNodeType type)
	{
		return
			type == ProjectNodeType.AssetFolder ||
			type == ProjectNodeType.AssetImage  ||
			type == ProjectNodeType.AssetMesh   ||
			type == ProjectNodeType.AssetBinary ||
			type == ProjectNodeType.AssetDefinition;
	}

	public static bool IsScriptsRelated(this ProjectNodeType type)
	{
		return
			type == ProjectNodeType.ScriptFolder ||
			type == ProjectNodeType.ScriptFile   ||
			type == ProjectNodeType.ScriptDefinition;
	}
}