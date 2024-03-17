using Veldrid;

namespace Mine.Framework;

public sealed class SharedMesh : IDisposable
{
	// geometry
	public DeviceBuffer?      VertexBuffer = null;
	public SharedVertexLayout SharedVertexLayout; // TODO - this needs to be shared, in reality almost every mesh in the game will have the same layout
	
	public DeviceBuffer? IndexBuffer       = null;
	public IndexFormat   IndexBufferFormat = IndexFormat.UInt16;
	public uint          IndexCount;

	public PrimitiveTopology Topology => PrimitiveTopology.TriangleList; // hardcoded for now
    
	public void Init(SceneMesh sceneMesh)
	{
		// create shared vertex layout
		byte[] vertexData = sceneMesh.BuildVertexBufferData(out VertexLayoutDescription vertexLayoutDescription);
		SharedVertexLayout = Engine.Shared.GetOrCreateVertexLayout(vertexLayoutDescription);
		
		// vertex buffer
		BufferDescription vertexBufferDescription = new
		(
			(uint)(sceneMesh.VertexCount * sceneMesh.VertexSize),
			BufferUsage.VertexBuffer
		);
		VertexBuffer = Engine.Graphics.Factory.CreateBuffer(vertexBufferDescription);

		Engine.Graphics.Device.UpdateBuffer(VertexBuffer, 0, vertexData);
		
		// index buffer
		if (sceneMesh.UShortIndices != null && sceneMesh.UShortIndices.Length > 0)
		{
			BufferDescription indexBufferDescription = new
			(
				(uint)(sizeof(ushort) * sceneMesh.UShortIndices.Length),
				BufferUsage.IndexBuffer
			);

			IndexBuffer       = Engine.Graphics.Factory.CreateBuffer(indexBufferDescription);
			IndexBufferFormat = IndexFormat.UInt16;
			IndexCount        = (uint)sceneMesh.UShortIndices.Length;

			Engine.Graphics.Device.UpdateBuffer(IndexBuffer, 0, sceneMesh.UShortIndices);
		}
		else if (sceneMesh.UIntIndices != null && sceneMesh.UIntIndices.Length > 0)
		{
			BufferDescription indexBufferDescription = new
			(
				(uint)(sizeof(ushort) * sceneMesh.UIntIndices.Length),
				BufferUsage.IndexBuffer
			);

			IndexBuffer       = Engine.Graphics.Factory.CreateBuffer(indexBufferDescription);
			IndexBufferFormat = IndexFormat.UInt32;
			IndexCount        = (uint)sceneMesh.UIntIndices.Length;

			Engine.Graphics.Device.UpdateBuffer(IndexBuffer, 0, sceneMesh.UIntIndices);
		}
	}
	
	public void Dispose()
	{
		VertexBuffer?.Dispose();
		IndexBuffer?.Dispose();
	}
}