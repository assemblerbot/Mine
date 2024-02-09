namespace Mine.Framework;

public enum ReferenceKind
{
	Uninitialized,
	
	AssetFolder,
	AssetImage,
	AssetMesh,
	AssetBinary,
	
	ScriptFolder,
	ScriptFile,
}

public static class ReferenceKindExtensions
{
	public static bool IsAssetsRelated(this ReferenceKind type)
	{
		return
			type == ReferenceKind.AssetFolder ||
			type == ReferenceKind.AssetImage  ||
			type == ReferenceKind.AssetMesh   ||
			type == ReferenceKind.AssetBinary;
	}

	public static bool IsScriptsRelated(this ReferenceKind type)
	{
		return
			type == ReferenceKind.ScriptFolder ||
			type == ReferenceKind.ScriptFile;
	}
}