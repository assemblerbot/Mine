using Veldrid;

namespace Mine.Framework;

public class MeshComponent : Component
{
	private BoundingBoxFloat _boundingBox;

	// geometry
	private DeviceBuffer?           _vertexBuffer = null;
	private DeviceBuffer?           _indexBuffer  = null;
	private VertexLayoutDescription _vertextLayoutDescription;
}