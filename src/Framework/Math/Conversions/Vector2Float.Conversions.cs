using System.Runtime.InteropServices;

namespace Mine.Framework;

public partial record struct Vector2Float
{
	[FieldOffset(0)] public System.Numerics.Vector2 NumericsVector2;
}