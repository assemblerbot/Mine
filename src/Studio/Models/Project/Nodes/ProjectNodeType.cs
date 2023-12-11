namespace RedHerring.Studio.Models.Project.FileSystem;

public enum ProjectNodeType
{
	Uninitialized,
	
	AssetFolder,
	AssetImage,
	AssetMesh,
	AssetBinary,
	AssetDefinitionData,
	
	ScriptFolder,
	ScriptFile,
	ScriptDefinitionTemplate,
}