namespace Mine.Framework;

public static class BinaryWriterExtensions
{
	public static void Write(this BinaryWriter writer, Point3Float point)
	{
		writer.Write(point.X);
		writer.Write(point.Y);
		writer.Write(point.Z);
	}

	public static void Write(this BinaryWriter writer, Vector3Float vector)
	{
		writer.Write(vector.X);
		writer.Write(vector.Y);
		writer.Write(vector.Z);
	}

	public static void Write(this BinaryWriter writer, Point4Float point)
	{
		writer.Write(point.X);
		writer.Write(point.Y);
		writer.Write(point.Z);
		writer.Write(point.W);
	}

	public static void Write(this BinaryWriter writer, Vector4Float vector)
	{
		writer.Write(vector.X);
		writer.Write(vector.Y);
		writer.Write(vector.Z);
		writer.Write(vector.W);
	}
	
	public static void Write(this BinaryWriter writer, Point2Float point)
	{
		writer.Write(point.X);
		writer.Write(point.Y);
	}

	public static void Write(this BinaryWriter writer, Vector2Float vector)
	{
		writer.Write(vector.X);
		writer.Write(vector.Y);
	}

	public static void Write(this BinaryWriter writer, Color4FloatRGBA color)
	{
		writer.Write(color.R);
		writer.Write(color.G);
		writer.Write(color.B);
		writer.Write(color.A);
	}
}