namespace Mine.Framework;

[Serializable]
public sealed class MeshReference : Reference
{
	public override ReferenceKind Kind => ReferenceKind.AssetMesh;

	//public          Mesh          ReferencedMesh;
}