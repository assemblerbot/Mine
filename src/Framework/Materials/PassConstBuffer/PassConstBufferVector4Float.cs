namespace Mine.Framework;

public class PassConstBufferVector4Float : PassConstBufferElement
{
	private Vector4Float _value;
	public Vector4Float Value
	{
		set
		{
			_value = value;
			SetDirty();
		}
	}

	public override int SizeInBytes => Vector4Float.SizeInBytes;

	public PassConstBufferVector4Float(string name, Vector4Float value)
		: base(name, PassConstBufferElementKind.Float4)
	{
		_value = value;
	}

	internal override void Write(BinaryWriter writer)
	{
		writer.Write(_value);
	}
}