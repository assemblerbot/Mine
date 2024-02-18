using System.Runtime.InteropServices;

namespace Mine.Framework;

public partial record struct Point3Float
{
	[NonSerialized, FieldOffset(0)] public System.Numerics.Vector3 NumericsVector3;
	[NonSerialized, FieldOffset(0)] public Vector3Float Vector3;
}