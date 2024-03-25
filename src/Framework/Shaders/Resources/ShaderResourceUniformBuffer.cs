using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceUniformBuffer : ShaderResource
{
	private uint          _sizeInBytes;
	private DeviceBuffer? _buffer;

	public ShaderResourceUniformBuffer(string name, uint sizeInBytes) : base(name, ResourceKind.UniformBuffer)
	{
		_sizeInBytes = sizeInBytes;
	}

	public BufferDescription CreateDescription()
	{
		return new BufferDescription(_sizeInBytes, BufferUsage.UniformBuffer);
	}

	public DeviceBuffer GetOrCreateBuffer()
	{
		if (_buffer is not null)
		{
			return _buffer;
		}

		_buffer = Engine.Graphics.Factory.CreateBuffer(CreateDescription());
		return _buffer;
	}

	public override BindableResource GetOrCreateBindableResource()
	{
		return GetOrCreateBuffer();
	}

	public override void Dispose()
	{
		_buffer?.Dispose();
		_buffer = null;
	}
}