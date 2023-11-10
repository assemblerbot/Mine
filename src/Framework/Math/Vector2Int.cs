using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameToolkit.Framework;

[StructLayout(LayoutKind.Explicit)]
public record struct Vector2Int(int x, int y)
{
	[FieldOffset(0)] public int x = x;
	[FieldOffset(0)] public int y = y;
	
	#region Constants
	public static Vector2Int Zero => new (0, 0);
	public static Vector2Int One  => new (1, 1);
	#endregion
	
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