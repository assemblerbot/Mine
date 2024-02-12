namespace Mine.Framework;

[Serializable]
public sealed class SceneMeshTextureCoordinateChannel
{
	// one of these
	public List<Point2Float>? UV;
	public List<Point3Float>? UVW;

	public int ItemSize => UV is not null ? Point2Float.Size : Point3Float.Size;
}