using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public record struct Vector4Float
{
	public const int SizeInBytes = 4 * sizeof(float);

	[FieldOffset(sizeof(float) * 0)] public float        X;
	[FieldOffset(sizeof(float) * 1)] public float        Y;
	[FieldOffset(sizeof(float) * 2)] public float        Z;
	[FieldOffset(sizeof(float) * 3)] public float        W;
	
	public static                           Vector4Float Zero = new Vector4Float(0, 0, 0, 0);

	public Vector4Float(float x, float y, float z, float w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}
}