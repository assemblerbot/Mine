using Veldrid;

namespace Mine.Framework;

public class MeshComponent : Component
{
	public BoundingBoxFloat BoundingBox;

	public SharedMesh? Mesh;
	public Material? Material;

	public MeshComponent(SharedMesh? mesh, Material? material)
	{
		Mesh     = mesh;
		Material = material;
	}

	public MeshComponent(SceneReference? sceneReference, int meshIndex, Material? material)
	{
		if (sceneReference is null)
		{
			Mesh = null;
		}
		else
		{
			Scene? scene = sceneReference.Value;
			Mesh = scene?.Meshes is null ? null : Engine.Shared.GetOrCreateMesh(sceneReference.Path, meshIndex, scene.Meshes[meshIndex]);
		}

		Material = material;
	}
}