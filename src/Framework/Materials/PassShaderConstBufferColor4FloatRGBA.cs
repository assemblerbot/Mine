namespace Mine.Framework;

public class PassShaderConstBufferColor4FloatRGBA : PassShaderConstBufferElement
{
	private Color4FloatRGBA _value;
	public Color4FloatRGBA Value
	{
		set
		{
			_value = value;
			SetDirty();
		}
	}

	public override int SizeInBytes => Color4FloatRGBA.SizeInBytes;

	public PassShaderConstBufferColor4FloatRGBA(string name, Color4FloatRGBA value)
		: base(name, ShaderConstBufferElementKind.Float4)
	{
		_value = value;
	}

	internal override void Write(BinaryWriter writer)
	{
		writer.Write(_value);
	}
}