using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameToolkit.Framework;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Vector2Float
{
	[FieldOffset(sizeof(float) * 0)] public float x;
	[FieldOffset(sizeof(float) * 1)] public float y;
	
	#region Constants
	public static Vector2Float Zero => new (0, 0);
	public static Vector2Float One  => new (1, 1);
	#endregion
	
	public Vector2Float(float x, float y)
	{
		this.x = x;
		this.y = y;
	}
	
	#region Operators
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Float operator +(Vector2Float a, Vector2Float b)
	{
		return new Vector2Float(a.x + b.x, a.y + b.y);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Float operator -(Vector2Float a, Vector2Float b)
	{
		return new Vector2Float(a.x - b.x, a.y - b.y);
	}
	#endregion
	
	#region Methods
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out float x, out float y)
	{
		x = this.x;
		y = this.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float SquareMagnitude()
	{
		return x * x + y * y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float Magnitude()
	{
		return MathF.Sqrt(SquareMagnitude());
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2Float Normalized()
	{
		float magnitude = Magnitude();
		return new Vector2Float(x / magnitude, y / magnitude);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalize()
	{
		float magnitude = Magnitude();
		x /= magnitude;
		y /= magnitude;
	}
	#endregion
}