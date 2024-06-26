using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public record struct TransformFloat
{
	public const int SizeInBytes = Point3Float.SizeInBytes + QuaternionFloat.SizeInBytes + Vector3Float.SizeInBytes;
	
	// local transformations
	[FieldOffset(sizeof(float) * 0)] public Vector3Float    Translation;
	[FieldOffset(sizeof(float) * 3)] public QuaternionFloat Rotation;
	[FieldOffset(sizeof(float) * 7)] public Vector3Float    Scale;

	public TransformFloat(Vector3Float translation, QuaternionFloat rotation, Vector3Float scale)
	{
		Translation = translation;
		Rotation    = rotation;
		Scale       = scale;
	}
}