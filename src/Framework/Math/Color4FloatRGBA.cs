using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public record struct Color4FloatRGBA
{
	public const int SizeInBytes = 4 * sizeof(float);
	
	[FieldOffset(sizeof(float) * 0)] public float R;
	[FieldOffset(sizeof(float) * 1)] public float G;
	[FieldOffset(sizeof(float) * 2)] public float B;
	[FieldOffset(sizeof(float) * 3)] public float A;

	public Color4FloatRGBA(float r, float g, float b, float a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}
}