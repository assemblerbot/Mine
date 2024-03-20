using Veldrid;

namespace Mine.Framework;

public class MeshComponent : Component, IRenderable
{
	public BoundingBoxFloat BoundingBox;

	private SharedMesh _sharedMesh;
	private Material   _material;
	private ulong      _renderMask;

	public SharedMesh SharedMesh => _sharedMesh;
	public Material   Material   => _material;
	public ulong      RenderMask => _renderMask;

	public MeshComponent(SharedMesh sharedMesh, Material material, ulong renderMask)
	{
		_sharedMesh = sharedMesh;
		_material   = material;
		_renderMask = renderMask;
	}

	public MeshComponent(SceneReference? sceneReference, int meshIndex, Material? material, ulong renderMask)
	{
		// TODO - empty object (cube), empty material
		if (sceneReference is null)
		{
			_sharedMesh = null;
		}
		else
		{
			Scene? scene = sceneReference.Value;
			_sharedMesh = scene?.Meshes is null ? null : Engine.Shared.GetOrCreateMesh(sceneReference.Path, meshIndex, scene.Meshes[meshIndex]);
		}

		_material   = material;
		_renderMask = renderMask;
	}
	public override void AfterAddedToWorld()
	{
		//Engine.World.RegisterMesh(TODO);
	}

	public override void BeforeRemovedFromWorld()
	{
		//Engine.World.UnregisterMesh(TODO);
	}

	public void Draw(CommandList commandList)
	{
		commandList.SetVertexBuffer(0, _sharedMesh.VertexBuffer); // TODO - slot index?
		commandList.SetIndexBuffer(_sharedMesh.IndexBuffer, _sharedMesh.IndexBufferFormat);
		commandList.DrawIndexed(_sharedMesh.IndexCount, 1, 0, 0, 0);
	}
}