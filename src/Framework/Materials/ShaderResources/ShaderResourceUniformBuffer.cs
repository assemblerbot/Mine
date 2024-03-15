using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceUniformBuffer : ShaderResource
{
	public readonly ShaderConstant[] Constants;

	private DeviceBuffer? _buffer;

	public ShaderResourceUniformBuffer(string name, ShaderStages stages, ShaderResourceStorage storage, params ShaderConstant[] constants) : base(name, ResourceKind.UniformBuffer, stages, storage)
	{
		Constants = constants;
	}

	public BufferDescription CreateDescription()
	{
		int sizeInBytes = Constants.Sum(constant => constant.SizeInBytes);
		return new BufferDescription((uint)sizeInBytes, BufferUsage.UniformBuffer);
	}

	public override BindableResource GetOrCreateBindableResource()
	{
		if (_buffer is not null)
		{
			return _buffer;
		}

		_buffer = Engine.Graphics.Factory.CreateBuffer(CreateDescription());
		return _buffer;
	}

	public override void Dispose()
	{
		_buffer?.Dispose();
		_buffer = null;
	}
}