using OdinSerializer;

namespace Mine.Framework;

[Serializable]
public class Reference
{
	public virtual ReferenceKind Kind => ReferenceKind.AssetBinary;

	public string Guid;
	public string ResourcePath;
}