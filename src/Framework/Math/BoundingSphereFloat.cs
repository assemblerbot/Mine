namespace Mine.Framework;

[Serializable]
public record struct BoundingSphereFloat
{
	public Point3Float Center;
	public float       Radius;
}