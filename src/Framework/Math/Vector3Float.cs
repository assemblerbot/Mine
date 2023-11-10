using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameToolkit.Framework;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Vector3Float(float x, float y, float z)
{
	[FieldOffset(0)] public float x = x;
	[FieldOffset(4)] public float y = y;
	[FieldOffset(8)] public float z = z;
	
	#region Constants
	public static Vector3Float Zero => new (0, 0, 0);
	public static Vector3Float One  => new (1, 1, 1);
	#endregion
	
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
