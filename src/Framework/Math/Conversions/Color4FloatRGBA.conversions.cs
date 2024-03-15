using System.Runtime.InteropServices;

namespace Mine.Framework;

public partial record struct Color4FloatRGBA
{
	[NonSerialized, FieldOffset(0)] public Veldrid.RgbaFloat VeldridRgbaFloat;
}