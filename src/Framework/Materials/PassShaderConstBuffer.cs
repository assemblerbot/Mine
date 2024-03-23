using Veldrid;

namespace Mine.Framework;

public sealed class PassShaderConstBuffer : IDisposable
{
	public readonly string                         Name;
	public readonly PassShaderConstBufferElement[] Elements;
	public readonly int                            SizeInBytes;
	
	public  ShaderStages Stages { get; internal set; }

	private bool          _isDirty = true;
	
	private DeviceBuffer? _constBuffer;
	public  DeviceBuffer  ConstBuffer
	{
		get
		{
			if (_constBuffer is not null)
			{
				return _constBuffer;
			}

			_constBuffer = Engine.Graphics.Factory.CreateBuffer(new BufferDescription((uint)SizeInBytes, BufferUsage.UniformBuffer));
			return _constBuffer;
		}
	}

	private ResourceLayout? _resourceLayout;
	public ResourceLayout ResourceLayout
	{
		get
		{
			_resourceLayout ??= Engine.Graphics.Factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription(Name, ResourceKind.UniformBuffer, Stages)
				)
			);
			
			return _resourceLayout;
		}
	}

	private ResourceSet? _resourceSet;
	public ResourceSet ResourceSet
	{
		get
		{
			_resourceSet ??= Engine.Graphics.Factory.CreateResourceSet(
				new ResourceSetDescription(
					ResourceLayout,
					new[] {ConstBuffer}
				)
			);

			if (_isDirty)
			{
				_isDirty = false;
				UpdateBuffer();
			}

			return _resourceSet;
		}
	}

	public PassShaderConstBuffer(string name, params PassShaderConstBufferElement[] elements)
	{
		Name     = name;
		Elements = elements;

		SizeInBytes = 0;
		foreach (PassShaderConstBufferElement element in elements)
		{
			element.SetDirty =  SetDirty;
			SizeInBytes      += element.SizeInBytes;
		}
	}
	
	public void Dispose()
	{
		_resourceLayout?.Dispose();
		_resourceSet?.Dispose();
		_constBuffer?.Dispose();
	}

	private void SetDirty()
	{
		_isDirty = true;
	}

	private void UpdateBuffer()
	{
		byte[] vertexData = new byte[SizeInBytes];
		{
			using MemoryStream memory = new (vertexData);
			using BinaryWriter writer = new (memory);
			
			foreach (PassShaderConstBufferElement element in Elements)
			{
				element.Write(writer);
			}

			writer.Flush();
		}

		Engine.Graphics.Device.UpdateBuffer(_constBuffer, 0, vertexData);
	}
}