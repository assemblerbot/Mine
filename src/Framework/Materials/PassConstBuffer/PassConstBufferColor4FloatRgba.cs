namespace Mine.Framework;

public class PassConstBufferColor4FloatRgba : PassConstBufferElement
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

	public PassConstBufferColor4FloatRgba(string name, Color4FloatRGBA value)
		: base(name, PassConstBufferElementKind.Float4)
	{
		_value = value;
	}

	internal override void Write(BinaryWriter writer)
	{
		writer.Write(_value);
	}
}