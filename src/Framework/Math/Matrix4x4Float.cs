using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public record struct Matrix4x4Float
{
	public const int Size = 16 * sizeof(float);
	
	[FieldOffset(sizeof(float) * 00)] public float        A0;
	[FieldOffset(sizeof(float) * 01)] public float        A1;
	[FieldOffset(sizeof(float) * 02)] public float        A2;
	[FieldOffset(sizeof(float) * 03)] public float        A3;
	[FieldOffset(sizeof(float) * 00)] public Vector4Float Row0;
	
	[FieldOffset(sizeof(float) * 04)] public float        B0;
	[FieldOffset(sizeof(float) * 05)] public float        B1;
	[FieldOffset(sizeof(float) * 06)] public float        B2;
	[FieldOffset(sizeof(float) * 07)] public float        B3;
	[FieldOffset(sizeof(float) * 04)] public Vector4Float Row1;
	
	[FieldOffset(sizeof(float) * 08)] public float        C0;
	[FieldOffset(sizeof(float) * 09)] public float        C1;
	[FieldOffset(sizeof(float) * 10)] public float        C2;
	[FieldOffset(sizeof(float) * 11)] public float        C3;
	[FieldOffset(sizeof(float) * 08)] public Vector4Float Row2;
	
	[FieldOffset(sizeof(float) * 12)] public float       D0;
	[FieldOffset(sizeof(float) * 13)] public float       D1;
	[FieldOffset(sizeof(float) * 14)] public float       D2;
	[FieldOffset(sizeof(float) * 15)] public float       D3;
	[FieldOffset(sizeof(float) * 12)] public Point4Float Row3;
	[FieldOffset(sizeof(float) * 12)] public Point4Float Position;
	[FieldOffset(sizeof(float) * 12)] public Vector4Float Translation;

	public float this[int i, int j]
	{
		get {
			switch (i)
			{
				case 0:
					switch (j)
					{
						case 0:  return A0;
						case 1:  return A1;
						case 2:  return A2;
						case 3:  return A3;
						default: return 0;
					}
				case 1:
					switch (j)
					{
						case 0:  return B0;
						case 1:  return B1;
						case 2:  return B2;
						case 3:  return B3;
						default: return 0;
					}
				case 2:
					switch (j)
					{
						case 0:  return C0;
						case 1:  return C1;
						case 2:  return C2;
						case 3:  return C3;
						default: return 0;
					}
				case 3:
					switch (j)
					{
						case 0:  return D0;
						case 1:  return D1;
						case 2:  return D2;
						case 3:  return D3;
						default: return 0;
					}
				default:
					return 0;
			}
		}
		set {
			switch (i)
			{
				case 0:
					switch (j)
					{
						case 0:  A0 = value; return;
						case 1:  A1 = value; return;
						case 2:  A2 = value; return;
						case 3:  A3 = value; return;
						default: return;
					}
				case 1:
					switch (j)
					{
						case 0:  B0 = value; return;
						case 1:  B1 = value; return;
						case 2:  B2 = value; return;
						case 3:  B3 = value; return;
						default: return;
					}
				case 2:
					switch (j)
					{
						case 0:  C0 = value; return;
						case 1:  C1 = value; return;
						case 2:  C2 = value; return;
						case 3:  C3 = value; return;
						default: return;
					}
				case 3:
					switch (j)
					{
						case 0:  D0 = value; return;
						case 1:  D1 = value; return;
						case 2:  D2 = value; return;
						case 3:  D3 = value; return;
						default: return;
					}
			}
		}
	}

	public Matrix4x4Float(
		float a0,
		float a1,
		float a2,
		float a3,
		float b0,
		float b1,
		float b2,
		float b3,
		float c0,
		float c1,
		float c2,
		float c3,
		float d0,
		float d1,
		float d2,
		float d3
		)
	{
		A0 = a0;
		A1 = a1;
		A2 = a2;
		A3 = a3;
		B0 = b0;
		B1 = b1;
		B2 = b2;
		B3 = b3;
		C0 = c0;
		C1 = c1;
		C2 = c2;
		C3 = c3;
		D0 = d0;
		D1 = d1;
		D2 = d2;
		D3 = d3;
	}

	// public void Decompose(out Point3Float translation, out QuaternionFloat rotation, out Vector3Float scale)
	// {
	// 	
	// }
}