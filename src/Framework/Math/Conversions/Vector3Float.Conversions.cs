using System.Runtime.InteropServices;

namespace GameToolkit.Framework;

public partial record struct Vector3Float
{
	[FieldOffset(0)] public System.Numerics.Vector3 NumericsVector3;
}