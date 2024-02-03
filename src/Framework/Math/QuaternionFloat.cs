using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public record struct QuaternionFloat
{
	public const int Size = 4 * sizeof(float);
	
	[FieldOffset(sizeof(float) * 0)] public float X;
	[FieldOffset(sizeof(float) * 1)] public float Y;
	[FieldOffset(sizeof(float) * 2)] public float Z;
	[FieldOffset(sizeof(float) * 3)] public float W;

	public QuaternionFloat(float x, float y, float z, float w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}
}