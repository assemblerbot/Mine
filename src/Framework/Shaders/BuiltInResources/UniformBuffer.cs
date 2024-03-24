using Veldrid;

namespace Mine.Framework.Shaders.BuiltInResources;

public abstract class UniformBuffer : IDisposable
{
	private DeviceBuffer? _buffer;
	public DeviceBuffer Buffer 
	{
		get
		{
			_buffer ??= Engine.Graphics.Factory.CreateBuffer(BufferDescription);
			return _buffer;
		}
	}

	public abstract BufferDescription BufferDescription { get; }

	public void Dispose()
	{
		_buffer?.Dispose();
	}
}