using Veldrid;

namespace Mine.Framework;

public sealed class SharedMesh : IDisposable
{
	// geometry
	private DeviceBuffer?      _vertexBuffer = null;
	private DeviceBuffer?      _indexBuffer  = null;
	private SharedVertexLayout _sharedVertexLayout; // TODO - this needs to be shared, in reality almost every mesh in the game will have the same layout

	public DeviceBuffer?      VertexBuffer       => _vertexBuffer;
	public DeviceBuffer?      IndexBuffer        => _indexBuffer;
	public SharedVertexLayout SharedVertexLayout => _sharedVertexLayout;

	public PrimitiveTopology Topology => PrimitiveTopology.TriangleList; // hardcoded for now
    
	public void Init(SceneMesh sceneMesh)
	{
		// create shared vertex layout
		byte[] vertexData = sceneMesh.BuildVertexBufferData(out VertexLayoutDescription vertexLayoutDescription);
		_sharedVertexLayout = Engine.Shared.GetOrCreateVertexLayout(vertexLayoutDescription);
		
		// vertex buffer
		BufferDescription vertexBufferDescription = new
		(
			(uint)(sceneMesh.VertexCount * sceneMesh.VertexSize),
			BufferUsage.VertexBuffer
		);
		_vertexBuffer = Engine.Graphics.Factory.CreateBuffer(vertexBufferDescription);

		Engine.Graphics.Device.UpdateBuffer(_vertexBuffer, 0, vertexData);
		
		// index buffer
		if (sceneMesh.UShortIndices != null && sceneMesh.UShortIndices.Length > 0)
		{
			BufferDescription indexBufferDescription = new
			(
				(uint)(sizeof(ushort) * sceneMesh.UShortIndices.Length),
				BufferUsage.IndexBuffer
			);

			_indexBuffer = Engine.Graphics.Factory.CreateBuffer(indexBufferDescription);

			Engine.Graphics.Device.UpdateBuffer(_indexBuffer, 0, sceneMesh.UShortIndices);
		}
		else if (sceneMesh.UIntIndices != null && sceneMesh.UIntIndices.Length > 0)
		{
			BufferDescription indexBufferDescription = new
			(
				(uint)(sizeof(ushort) * sceneMesh.UIntIndices.Length),
				BufferUsage.IndexBuffer
			);

			_indexBuffer = Engine.Graphics.Factory.CreateBuffer(indexBufferDescription);

			Engine.Graphics.Device.UpdateBuffer(_indexBuffer, 0, sceneMesh.UIntIndices);
		}
	}
	
	public void Dispose()
	{
		_vertexBuffer?.Dispose();
		_indexBuffer?.Dispose();
	}
}