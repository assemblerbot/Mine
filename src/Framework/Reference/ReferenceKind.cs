namespace Mine.Framework;

public enum ReferenceKind
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

public static class ReferenceKindExtensions
{
	public static bool IsAssetsRelated(this ReferenceKind type)
	{
		return
			type == ReferenceKind.AssetFolder ||
			type == ReferenceKind.AssetImage  ||
			type == ReferenceKind.AssetMesh   ||
			type == ReferenceKind.AssetBinary ||
			type == ReferenceKind.AssetDefinition;
	}

	public static bool IsScriptsRelated(this ReferenceKind type)
	{
		return
			type == ReferenceKind.ScriptFolder ||
			type == ReferenceKind.ScriptFile   ||
			type == ReferenceKind.ScriptDefinition;
	}
}