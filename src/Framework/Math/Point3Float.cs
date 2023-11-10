using System.Runtime.InteropServices;

namespace GameToolkit.Framework;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Point3Float(float x, float y, float z)
{
	[FieldOffset(0)] public float x = x;
	[FieldOffset(4)] public float y = y;
	[FieldOffset(8)] public float z = z;

	public void Deconstruct(out float x, out float y, out float z)
	{
		x = this.x;
		y = this.y;
		z = this.z;
	}
	
	
}