using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/vectorization-guidelines.md#code-structure

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public partial record struct Vector3Float
{
	[FieldOffset(sizeof(float) * 0)] public float X;
	[FieldOffset(sizeof(float) * 1)] public float Y;
	[FieldOffset(sizeof(float) * 2)] public float Z;
	
	#region Constants
	public static Vector3Float Zero => new (0, 0, 0);
	public static Vector3Float One  => new (1, 1, 1);
	#endregion
	
	public Vector3Float(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}
	
	#region Operators
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Float operator +(Vector3Float a, Vector3Float b)
	{
		return new Vector3Float(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Float operator -(Vector3Float a, Vector3Float b)
	{
		return new Vector3Float(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}
	#endregion
	
	#region Methods
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out float x, out float y, out float z)
	{
		x = this.X;
		y = this.Y;
		z = this.Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float SquareMagnitude()
	{
		return X * X + Y * Y + Z * Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float Magnitude()
	{
		return MathF.Sqrt(SquareMagnitude());
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3Float Normalized()
	{
		float magnitude = Magnitude();
		return new Vector3Float(X / magnitude, Y / magnitude, Z / magnitude);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalize()
	{
		float magnitude = Magnitude();
		X /= magnitude;
		Y /= magnitude;
		Z /= magnitude;
	}
	#endregion
}
