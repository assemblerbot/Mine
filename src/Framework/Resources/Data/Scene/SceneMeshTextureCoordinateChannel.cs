namespace Mine.Framework;

[Serializable]
public sealed class SceneMeshTextureCoordinateChannel
{
	// one of these
	public List<Point2Float>? UV;
	public List<Point3Float>? UVW;

	public int ItemSizeInBytes => UV is not null ? Point2Float.SizeInBytes : Point3Float.SizeInBytes;
}