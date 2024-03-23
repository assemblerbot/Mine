using Veldrid;

namespace Mine.Framework;

public abstract class PassConstBufferElement
{
	public readonly string                       Name;
	public readonly PassConstBufferElementKind Kind;
	public abstract int                          SizeInBytes { get; }

	public Action SetDirty { get; internal set; }

	protected PassConstBufferElement(string name, PassConstBufferElementKind kind)
	{
		Name = name;
		Kind = kind;
	}

	internal abstract void Write(BinaryWriter writer);
}