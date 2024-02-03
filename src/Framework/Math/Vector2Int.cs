using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public record struct Vector2Int
{
	public const int Size = 2 * sizeof(int);
	
	[FieldOffset(sizeof(int) * 0)] public int X;
	[FieldOffset(sizeof(int) * 1)] public int Y;
	
	#region Constants
	public static Vector2Int Zero => new (0, 0);
	public static Vector2Int One  => new (1, 1);
	#endregion
	
	public Vector2Int(int x, int y)
	{
		X = x;
		Y = y;
	}
	
	#region Operators
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator +(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.X + b.X, a.Y + b.Y);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator -(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.X - b.X, a.Y - b.Y);
	}
	#endregion
	
	#region Methods
	
	#endregion
}