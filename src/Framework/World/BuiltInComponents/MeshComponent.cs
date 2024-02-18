using Veldrid;

namespace Mine.Framework;

public class MeshComponent : Component
{
	private BoundingBoxFloat _boundingBox;

	private SharedMesh? _mesh;
	private Material? _material = null;

	public void Init(SharedMesh mesh, Material material)
	{
		_mesh     = mesh;
		_material = material;
	}
}