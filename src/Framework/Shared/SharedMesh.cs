using Veldrid;

namespace Mine.Framework;

public sealed class SharedMesh : IDisposable
{
	// geometry
	private DeviceBuffer?           _vertexBuffer = null;
	private DeviceBuffer?           _indexBuffer  = null;
	private VertexLayoutDescription _vertextLayoutDescription;

	public DeviceBuffer?           VertexBuffer            => _vertexBuffer;
	public DeviceBuffer?           IndexBuffer             => _indexBuffer;
	public VertexLayoutDescription VertexLayoutDescription => _vertextLayoutDescription;
    
	public void Init(SceneMesh sceneMesh)
	{
		byte[] vertexData = sceneMesh.BuildVertexBufferData(out _vertextLayoutDescription);
		
		// vertex buffer
		BufferDescription vertexBufferDescription = new
		(
			(uint)(sceneMesh.VertexCount * sceneMesh.VertexSize),
			BufferUsage.VertexBuffer
		);
		_vertexBuffer = Engine.Renderer.Factory.CreateBuffer(vertexBufferDescription);

		Engine.Renderer.Device.UpdateBuffer(_vertexBuffer, 0, vertexData);
		
		// index buffer
		if (sceneMesh.UShortIndices != null && sceneMesh.UShortIndices.Length > 0)
		{
			BufferDescription indexBufferDescription = new
			(
				(uint)(sizeof(ushort) * sceneMesh.UShortIndices.Length),
				BufferUsage.IndexBuffer
			);

			_indexBuffer = Engine.Renderer.Factory.CreateBuffer(indexBufferDescription);

			Engine.Renderer.Device.UpdateBuffer(_indexBuffer, 0, sceneMesh.UShortIndices);
		}
		else if (sceneMesh.UIntIndices != null && sceneMesh.UIntIndices.Length > 0)
		{
			BufferDescription indexBufferDescription = new
			(
				(uint)(sizeof(ushort) * sceneMesh.UIntIndices.Length),
				BufferUsage.IndexBuffer
			);

			_indexBuffer = Engine.Renderer.Factory.CreateBuffer(indexBufferDescription);

			Engine.Renderer.Device.UpdateBuffer(_indexBuffer, 0, sceneMesh.UIntIndices);
		}
	}
	
	public void Dispose()
	{
		_vertexBuffer?.Dispose();
		_indexBuffer?.Dispose();
	}
}