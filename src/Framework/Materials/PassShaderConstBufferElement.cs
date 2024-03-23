using Veldrid;

namespace Mine.Framework;

public abstract class PassShaderConstBufferElement
{
	public readonly string                       Name;
	public readonly ShaderConstBufferElementKind Kind;
	public abstract int                          SizeInBytes { get; }

	public Action SetDirty { get; internal set; }

	protected PassShaderConstBufferElement(string name, ShaderConstBufferElementKind kind)
	{
		Name = name;
		Kind = kind;
	}

	internal abstract void Write(BinaryWriter writer);
}