using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public partial record struct Color4FloatRGBA
{
	public const int SizeInBytes = 4 * sizeof(float);
	
	[FieldOffset(sizeof(float) * 0)] public float R;
	[FieldOffset(sizeof(float) * 1)] public float G;
	[FieldOffset(sizeof(float) * 2)] public float B;
	[FieldOffset(sizeof(float) * 3)] public float A;

	public static Color4FloatRGBA Transparent = new (0, 0, 0, 0);
	public static Color4FloatRGBA Black       = new (0, 0, 0, 1);
	public static Color4FloatRGBA White       = new (1, 1, 1, 1);
	public static Color4FloatRGBA Red         = new (1, 0, 0, 1);
	public static Color4FloatRGBA Green       = new (0, 1, 0, 1);
	public static Color4FloatRGBA Blue        = new (0, 0, 1, 1);
	
	public Color4FloatRGBA(float r, float g, float b, float a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}
}