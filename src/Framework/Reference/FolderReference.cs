namespace Mine.Framework;

[Serializable]
public sealed class FolderReference : Reference
{
	public override ReferenceKind Kind => ReferenceKind.AssetFolder;
}