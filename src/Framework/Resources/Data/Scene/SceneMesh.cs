namespace Mine.Framework;

[Serializable]
public sealed class SceneMesh
{
	public string           Name = "";
	public BoundingBoxFloat BoundingBox;
	public List<Point3Float>?                       Positions;
	public List<Vector3Float>?                      Normals;
	public List<Vector3Float>?                      Tangents;
	public List<Vector3Float>?                      BiTangents;
	public List<ushort>?                            UShortIndices;
	public List<uint>?                              UIntIndices;
	public List<SceneMeshTextureCoordinateChannel>? TextureCoordinateChannels;
	public List<SceneMeshVertexColorChannel>?       VertexColorChannels;
}