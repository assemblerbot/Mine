using Veldrid;

namespace Mine.Framework;

public sealed class PassConstBuffer : IDisposable
{
	public readonly string                   Name;
	public readonly ShaderResourceSetKind    Kind;
	public readonly PassConstBufferElement[] Elements;
	public readonly int                      SizeInBytes;
	
	private bool _isDirty = true;
	
	private DeviceBuffer? _constBuffer;
	public DeviceBuffer ConstBuffer
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

	private readonly MultiStageResourceLayout _resourceLayout;
	private readonly MultiStageResourceSet    _resourceSet;

	public PassConstBuffer(string name, ShaderResourceSetKind kind, params PassConstBufferElement[] elements)
	{
		Name     = name;
		Kind     = kind;
		Elements = elements;

		SizeInBytes = 0;
		foreach (PassConstBufferElement element in elements)
		{
			element.SetDirty =  SetDirty;
			SizeInBytes      += element.SizeInBytes;
		}
		
		_resourceLayout = new MultiStageResourceLayout(GetResourceLayoutDescription);
		_resourceSet    = new MultiStageResourceSet(GetResourceSetDescription);
	}

	public void Dispose()
	{
		_resourceLayout.Dispose();
		_resourceSet.Dispose();
		_constBuffer?.Dispose();
	}

	private ResourceLayoutDescription GetResourceLayoutDescription(ShaderStages stages)
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription(Name, ResourceKind.UniformBuffer, stages)
		);
	}

	private ResourceSetDescription GetResourceSetDescription(ShaderStages stages)
	{
		return new ResourceSetDescription(
			GetResourceLayout(stages),
			new[] {ConstBuffer}
		);
	}

	public ResourceLayout GetResourceLayout(ShaderStages stages)
	{
		return _resourceLayout.Get(stages);
	}

	public ResourceSet GetResourceSet(ShaderStages stages)
	{
		if (_isDirty)
		{
			_isDirty = false;
			UpdateBuffer();
		}

		return _resourceSet.Get(stages);
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
			
			foreach (PassConstBufferElement element in Elements)
			{
				element.Write(writer);
			}

			writer.Flush();
		}

		Engine.Graphics.Device.UpdateBuffer(ConstBuffer, 0, vertexData);
	}
}