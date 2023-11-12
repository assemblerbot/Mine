using System.Runtime.InteropServices;

namespace Mine.Framework;

public partial record struct Point3Float
{
	[FieldOffset(0)] public System.Numerics.Vector3 NumericsVector3;
}