namespace Mine.Framework;

[Serializable]
public record struct BoundingBoxFloat
{
	public const int Size = 2 * Point3Float.Size;
	
	public Point3Float Min;
	public Point3Float Max;

	public static BoundingBoxFloat Empty => new (new Point3Float(float.MaxValue), new Point3Float(float.MinValue));

	public BoundingBoxFloat(Point3Float min, Point3Float max)
	{
		Min = min;
		Max = max;
	}
}