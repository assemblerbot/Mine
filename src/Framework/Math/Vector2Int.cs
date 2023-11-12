using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mine.Framework;

[StructLayout(LayoutKind.Explicit)]
public record struct Vector2Int
{
	[FieldOffset(sizeof(int) * 0)] public int x;
	[FieldOffset(sizeof(int) * 1)] public int y;
	
	#region Constants
	public static Vector2Int Zero => new (0, 0);
	public static Vector2Int One  => new (1, 1);
	#endregion
	
	public Vector2Int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	#region Operators
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator +(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x + b.x, a.y + b.y);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator -(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x - b.x, a.y - b.y);
	}
	#endregion
	
	#region Methods
	
	#endregion
}