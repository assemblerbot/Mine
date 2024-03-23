namespace Mine.Framework;

public class PassShaderConstBufferVector4Float : PassShaderConstBufferElement
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

	public PassShaderConstBufferVector4Float(string name, Vector4Float value)
		: base(name, ShaderConstBufferElementKind.Float4)
	{
		_value = value;
	}

	internal override void Write(BinaryWriter writer)
	{
		writer.Write(_value);
	}
}