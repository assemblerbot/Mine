using System.Runtime.InteropServices;

namespace Mine.Framework;

public partial record struct Vector3Float
{
	[NonSerialized, FieldOffset(0)] public System.Numerics.Vector3 NumericsVector3;

	public Point3Float Point3 => new (X, Y, Z);
}