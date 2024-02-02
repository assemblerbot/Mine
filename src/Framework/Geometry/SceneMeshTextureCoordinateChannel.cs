namespace Mine.Framework;

[Serializable]
public sealed class SceneMeshTextureCoordinateChannel
{
	// one of these
	public List<Point2Float>? UV;
	public List<Point3Float>? UVW;
}