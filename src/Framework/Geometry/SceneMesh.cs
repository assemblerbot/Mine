namespace Mine.Framework;

[Serializable]
public sealed class SceneMesh
{
	public List<Point3Float>?  Vertices;
	public List<Vector3Float>? Normals;
	public List<Vector3Float>? Tangents;
	public List<Vector3Float>? BiTangents;
	public ushort[]?           UShortIndices;
	public uint[]?             UIntIndices;
}