namespace GameToolkit.Framework;

public record struct Int2(int x, int y)
{
	public int x = x;
	public int y = y;
	
	public Int2() : this(0, 0)
	{
	}
	
	#region Constants
	public static Int2 Zero => new (0, 0);
	public static Int2 One  => new (1, 1);
	#endregion
	
	#region Operators
	public static Int2 operator +(Int2 a, Int2 b)
	{
		return new Int2(a.x + b.x, a.y + b.y);
	}
	
	public static Int2 operator -(Int2 a, Int2 b)
	{
		return new Int2(a.x - b.x, a.y - b.y);
	}
	#endregion
	
	#region Methods
	
	#endregion
}