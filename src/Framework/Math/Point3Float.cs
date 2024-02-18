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
}