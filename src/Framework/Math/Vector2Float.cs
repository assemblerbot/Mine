using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mine.Framework;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Vector2Float
{
	[FieldOffset(sizeof(float) * 0)] public float X;
	[FieldOffset(sizeof(float) * 1)] public float Y;
	
	#region Constants
	public static Vector2Float Zero => new (0, 0);
	public static Vector2Float One  => new (1, 1);
	#endregion
	
	public Vector2Float(float x, float y)
	{
		X = x;
		Y = y;
	}
	
	#region Operators
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Float operator +(Vector2Float a, Vector2Float b)
	{
		return new Vector2Float(a.X + b.X, a.Y + b.Y);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Float operator -(Vector2Float a, Vector2Float b)
	{
		return new Vector2Float(a.X - b.X, a.Y - b.Y);
	}
	#endregion
	
	#region Methods
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out float x, out float y)
	{
		x = this.X;
		y = this.Y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float SquareMagnitude()
	{
		return X * X + Y * Y;
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
		return new Vector2Float(X / magnitude, Y / magnitude);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalize()
	{
		float magnitude = Magnitude();
		X /= magnitude;
		Y /= magnitude;
	}
	#endregion
}