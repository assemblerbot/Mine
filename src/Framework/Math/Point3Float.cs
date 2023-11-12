using System.Runtime.InteropServices;

namespace Mine.Framework;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Point3Float
{
	[FieldOffset(sizeof(float) * 0)] public float x;
	[FieldOffset(sizeof(float) * 1)] public float y;
	[FieldOffset(sizeof(float) * 2)] public float z;

	public Point3Float(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}
	
	public void Deconstruct(out float x, out float y, out float z)
	{
		x = this.x;
		y = this.y;
		z = this.z;
	}
	
	
}