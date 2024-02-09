namespace Mine.Framework;

[Serializable]
public sealed class FolderReference : Reference
{
	public FolderReference(string path) : base(path)
	{
	}
}