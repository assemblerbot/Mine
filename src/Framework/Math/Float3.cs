using System.Runtime.InteropServices;

namespace GameToolkit.Framework;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Float3(float x, float y, float z)
{
	[FieldOffset(0)] public float x = x;
	[FieldOffset(4)] public float y = y;
	[FieldOffset(8)] public float z = z;
}

public partial record struct Float3
{
	[FieldOffset(0)] public System.Numerics.Vector3 NumericsVector3;
}