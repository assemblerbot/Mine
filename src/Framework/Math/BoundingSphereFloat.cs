namespace Mine.Framework;

[Serializable]
public record struct BoundingSphereFloat
{
	public const int Size = Point3Float.SizeInBytes + sizeof(float);
	
	public Point3Float Center;
	public float       Radius;
}