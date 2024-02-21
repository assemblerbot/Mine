using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public partial record struct Point3Float
{
	public const int SizeInBytes = 3 * sizeof(float);
	
	[FieldOffset(sizeof(float) * 0)] public float X;
	[FieldOffset(sizeof(float) * 1)] public float Y;
	[FieldOffset(sizeof(float) * 2)] public float Z;

	public static Point3Float Zero = new (0, 0, 0);
	
	public Point3Float(float xyz)
	{
		X = xyz;
		Y = xyz;
		Z = xyz;
	}

	public Point3Float(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public void Deconstruct(out float x, out float y, out float z)
	{
		x = X;
		y = Y;
		z = Z;
	}
	
	#region Operators
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point3Float operator +(Point3Float a, Vector3Float b)
	{
		return new Point3Float(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point3Float operator +(Vector3Float a, Point3Float b)
	{
		return new Point3Float(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Float operator -(Point3Float a, Point3Float b)
	{
		return new Vector3Float(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}
	#endregion
	
	#region Transformations
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Point3Float Transform(Matrix4x4Float matrix)
	{
		return new Point3Float(
			X * matrix.M11 + Y * matrix.M21 + Z * matrix.M31 + matrix.M41,
			X * matrix.M12 + Y * matrix.M22 + Z * matrix.M32 + matrix.M42,
			X * matrix.M13 + Y * matrix.M23 + Z * matrix.M33 + matrix.M43);
	}
	#endregion

	public override string ToString()
	{
		return $"{X},{Y},{Z}";
	}
}