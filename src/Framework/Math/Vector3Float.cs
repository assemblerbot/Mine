using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/vectorization-guidelines.md#code-structure

namespace Mine.Framework;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Vector3Float
{
	[FieldOffset(sizeof(float) * 0)] public float x;
	[FieldOffset(sizeof(float) * 1)] public float y;
	[FieldOffset(sizeof(float) * 2)] public float z;
	
	#region Constants
	public static Vector3Float Zero => new (0, 0, 0);
	public static Vector3Float One  => new (1, 1, 1);
	#endregion
	
	public Vector3Float(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}
	
	#region Operators
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Float operator +(Vector3Float a, Vector3Float b)
	{
		return new Vector3Float(a.x + b.x, a.y + b.y, a.z + b.z);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Float operator -(Vector3Float a, Vector3Float b)
	{
		return new Vector3Float(a.x - b.x, a.y - b.y, a.z - b.z);
	}
	#endregion
	
	#region Methods
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out float x, out float y, out float z)
	{
		x = this.x;
		y = this.y;
		z = this.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float SquareMagnitude()
	{
		return x * x + y * y + z * z;
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
		return new Vector3Float(x / magnitude, y / magnitude, z / magnitude);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalize()
	{
		float magnitude = Magnitude();
		x /= magnitude;
		y /= magnitude;
		z /= magnitude;
	}
	#endregion
}
