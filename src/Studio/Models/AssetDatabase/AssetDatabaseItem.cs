namespace Mine.Studio;

[Serializable]
public sealed class AssetDatabaseItem
{
	public string Guid   = null!;
	public string Path   = null!;
	public bool   IsFile = true; // false means folder

	public AssetDatabaseItem()
	{
	}

	public AssetDatabaseItem(string guid, string path, bool isFile)
	{
		Guid   = guid;
		Path   = path;
		IsFile = isFile;
	}
}