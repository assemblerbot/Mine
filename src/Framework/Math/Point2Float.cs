using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public partial record struct Point2Float
{
	public const int SizeInBytes = 2 * sizeof(float);
	
	[FieldOffset(sizeof(float) * 0)] public float X;
	[FieldOffset(sizeof(float) * 1)] public float Y;

	public Point2Float(float x, float y)
	{
		X = x;
		Y = y;
	}
	
	public void Deconstruct(out float x, out float y)
	{
		x = X;
		y = Y;
	}
	
	
}