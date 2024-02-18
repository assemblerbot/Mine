using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public record struct Matrix4x4Float
{
	public const int Size = 16 * sizeof(float);
	
	[FieldOffset(sizeof(float) * 00)] public float        M11;
	[FieldOffset(sizeof(float) * 01)] public float        M12;
	[FieldOffset(sizeof(float) * 02)] public float        M13;
	[FieldOffset(sizeof(float) * 03)] public float        M14;
	[FieldOffset(sizeof(float) * 00)] public Vector4Float Row0;
	[FieldOffset(sizeof(float) * 00)] public Vector3Float AxisX;
	
	[FieldOffset(sizeof(float) * 04)] public float        M21;
	[FieldOffset(sizeof(float) * 05)] public float        M22;
	[FieldOffset(sizeof(float) * 06)] public float        M23;
	[FieldOffset(sizeof(float) * 07)] public float        M24;
	[FieldOffset(sizeof(float) * 04)] public Vector4Float Row1;
	[FieldOffset(sizeof(float) * 04)] public Vector3Float AxisY;
	
	[FieldOffset(sizeof(float) * 08)] public float        M31;
	[FieldOffset(sizeof(float) * 09)] public float        M32;
	[FieldOffset(sizeof(float) * 10)] public float        M33;
	[FieldOffset(sizeof(float) * 11)] public float        M34;
	[FieldOffset(sizeof(float) * 08)] public Vector4Float Row2;
	[FieldOffset(sizeof(float) * 08)] public Vector3Float AxisZ;
	
	[FieldOffset(sizeof(float) * 12)] public float        M41;
	[FieldOffset(sizeof(float) * 13)] public float        M42;
	[FieldOffset(sizeof(float) * 14)] public float        M43;
	[FieldOffset(sizeof(float) * 15)] public float        M44;
	[FieldOffset(sizeof(float) * 12)] public Vector4Float Row3;
	[FieldOffset(sizeof(float) * 12)] public Point3Float  Position;
	[FieldOffset(sizeof(float) * 12)] public Vector3Float Translation;

	public float this[int i, int j]
	{
		get {
			switch (i)
			{
				case 0:
					switch (j)
					{
						case 0:  return M11;
						case 1:  return M12;
						case 2:  return M13;
						case 3:  return M14;
						default: return 0;
					}
				case 1:
					switch (j)
					{
						case 0:  return M21;
						case 1:  return M22;
						case 2:  return M23;
						case 3:  return M24;
						default: return 0;
					}
				case 2:
					switch (j)
					{
						case 0:  return M31;
						case 1:  return M32;
						case 2:  return M33;
						case 3:  return M34;
						default: return 0;
					}
				case 3:
					switch (j)
					{
						case 0:  return M41;
						case 1:  return M42;
						case 2:  return M43;
						case 3:  return M44;
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
						case 0:  M11 = value; return;
						case 1:  M12 = value; return;
						case 2:  M13 = value; return;
						case 3:  M14 = value; return;
						default: return;
					}
				case 1:
					switch (j)
					{
						case 0:  M21 = value; return;
						case 1:  M22 = value; return;
						case 2:  M23 = value; return;
						case 3:  M24 = value; return;
						default: return;
					}
				case 2:
					switch (j)
					{
						case 0:  M31 = value; return;
						case 1:  M32 = value; return;
						case 2:  M33 = value; return;
						case 3:  M34 = value; return;
						default: return;
					}
				case 3:
					switch (j)
					{
						case 0:  M41 = value; return;
						case 1:  M42 = value; return;
						case 2:  M43 = value; return;
						case 3:  M44 = value; return;
						default: return;
					}
			}
		}
	}

	public static Matrix4x4Float Identity = new
	(
		1, 0, 0, 0,
		0, 1, 0, 0,
		0, 0, 1, 0,
		0, 0, 0, 1
	);

	#region Creation
	public Matrix4x4Float(
		float m11, float m12, float m13, float m14,
		float m21, float m22, float m23, float m24,
		float m31, float m32, float m33, float m34,
		float m41, float m42, float m43, float m44
	)
	{
		M11 = m11; M12 = m12; M13 = m13; M14 = m14;
		M21 = m21; M22 = m22; M23 = m23; M24 = m24;
		M31 = m31; M32 = m32; M33 = m33; M34 = m34;
		M41 = m41; M42 = m42; M43 = m43; M44 = m44;
	}

	// public void Decompose(out Point3Float translation, out QuaternionFloat rotation, out Vector3Float scale)
	// {
	// 	
	// }

	public static Matrix4x4Float CreateTranslationMatrix(Vector3Float translation)
	{
		Matrix4x4Float m = Matrix4x4Float.Identity;
		m.Translation = translation;
		return m;
	}

	public static Matrix4x4Float CreateViewLookAtLH(Point3Float eye, Point3Float at, Vector3Float up)
	{
		Vector3Float zAxis = at - eye; zAxis.Normalize();
		Vector3Float xAxis = Vector3Float.Cross(up, zAxis);	xAxis.Normalize();
		Vector3Float yAxis = Vector3Float.Cross(zAxis, xAxis);

		Vector3Float eyeVector = eye.Vector3;
		
		return new Matrix4x4Float
		       {
			       M11 = xAxis.X, M12 = yAxis.X, M13 = zAxis.X, M14 = 0,
			       M21 = xAxis.Y, M22 = yAxis.Y, M23 = zAxis.Y, M24 = 0,
			       M31 = xAxis.Z, M32 = yAxis.Z, M33 = zAxis.Z, M34 = 0,
			       M41 = -Vector3Float.Dot(xAxis, eyeVector), M42 = -Vector3Float.Dot(yAxis, eyeVector), M43 = -Vector3Float.Dot(zAxis, eyeVector), M44 = 1,
		       };
	}

	public static Matrix4x4Float CreateProjectionPerspectiveLH(float fovY, float aspectHDivW, float nearZ, float farZ)
	{
		float h = MathF.Cos(fovY /2) / MathF.Sin(fovY /2); // = cot()
		float w = h             / aspectHDivW;

		float z1 = farZ         /(farZ - nearZ);
		float z2 = -nearZ *farZ /(farZ - nearZ);

		return new Matrix4x4Float
		       {
			       M11 = w, M12 = 0, M13 = 0, M14 = 0,
			       M21 = 0, M22 = h, M23 = 0, M24 = 0,
			       M31 = 0, M32 = 0, M33 = z1, M34 = 1,
			       M41 = 0, M42 = 0, M43 = z2, M44 = 0,
		       };
	}

	public static Matrix4x4Float CreateProjectionOrthogonalLH(float w, float h, float nearZ, float farZ)
	{
		float z1 = 1      /(farZ - nearZ);
		float z2 = -nearZ /(farZ - nearZ);

		return new Matrix4x4Float
		       {
			       M11 = 2 / w, M12 = 0, M13 = 0, M14 = 0,
			       M21 = 0, M22 = 2 /h, M23 = 0, M24 = 0,
			       M31 = 0, M32 = 0, M33 = z1, M34 = 0,
			       M41 = 0, M42 = 0, M43 = z2, M44 = 1,
		       };
	}

	#endregion
	
	#region Operations
	public static Matrix4x4Float Mul(Matrix4x4Float m1, Matrix4x4Float m2)
	{
		return new Matrix4x4Float
		       {
			       M11 = m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31 + m1.M14 * m2.M41,
			       M12 = m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32 + m1.M14 * m2.M42,
			       M13 = m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33 + m1.M14 * m2.M43,
			       M14 = m1.M11 * m2.M14 + m1.M12 * m2.M24 + m1.M13 * m2.M34 + m1.M14 * m2.M44,

			       M21 = m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31 + m1.M24 * m2.M41,
			       M22 = m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32 + m1.M24 * m2.M42,
			       M23 = m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33 + m1.M24 * m2.M43,
			       M24 = m1.M21 * m2.M14 + m1.M22 * m2.M24 + m1.M23 * m2.M34 + m1.M24 * m2.M44,

			       M31 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31 + m1.M34 * m2.M41,
			       M32 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32 + m1.M34 * m2.M42,
			       M33 = m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33 + m1.M34 * m2.M43,
			       M34 = m1.M31 * m2.M14 + m1.M32 * m2.M24 + m1.M33 * m2.M34 + m1.M34 * m2.M44,

			       M41 = m1.M41 * m2.M11 + m1.M42 * m2.M21 + m1.M43 * m2.M31 + m1.M44 * m2.M41,
			       M42 = m1.M41 * m2.M12 + m1.M42 * m2.M22 + m1.M43 * m2.M32 + m1.M44 * m2.M42,
			       M43 = m1.M41 * m2.M13 + m1.M42 * m2.M23 + m1.M43 * m2.M33 + m1.M44 * m2.M43,
			       M44 = m1.M41 * m2.M14 + m1.M42 * m2.M24 + m1.M43 * m2.M34 + m1.M44 * m2.M44,
		       };
	}
	#endregion
	
	#region Row/Column access
	public Vector4Float GetRow(int row)
	{
		return row switch
		{
			0 => Row0,
			1 => Row1,
			2 => Row2,
			3 => Row3,
			_ => Vector4Float.Zero
		};
	}

	public void SetRow(int row, Vector4Float value)
	{
		switch (row)
		{
			case 0:
				Row0 = value;
				return;
			case 1:
				Row1 = value;
				return;
			case 2:
				Row2 = value;
				return;
			case 3:
				Row3 = value;
				return;
		}
	}

	public Vector4Float GetColumn(int column)
	{
		return column switch
		{
			0 => new Vector4Float(M11, M21, M31, M41),
			1 => new Vector4Float(M12, M22, M32, M42),
			2 => new Vector4Float(M13, M23, M33, M43),
			3 => new Vector4Float(M14, M24, M34, M44),
			_ => Vector4Float.Zero
		};
	}

	public void SetColumn(int column, Vector4Float value)
	{
		switch (column)
		{
			case 0:
				M11 = value.X;
				M21 = value.Y;
				M31 = value.Z;
				M41 = value.W;
				break;
			case 1:
				M12 = value.X;
				M22 = value.Y;
				M32 = value.Z;
				M42 = value.W;
				break;
			case 2:
				M13 = value.X;
				M23 = value.Y;
				M33 = value.Z;
				M43 = value.W;
				break;
			case 3:
				M14 = value.X;
				M24 = value.Y;
				M34 = value.Z;
				M44 = value.W;
				break;
		}
	}
	#endregion
	
	#region Scale
	public void SetScale(Vector3Float scale)
	{
		M11 = scale.X;
		M22 = scale.Y;
		M33 = scale.Z;
	}
	#endregion
	
	#region Rotation
	public void SetRotationX(float angle)
	{
		float s = MathF.Sin(angle);
		float c = MathF.Cos(angle);

		M11 = 1; M12 = 0;  M13 = 0; M14 = 0;
		M21 = 0; M22 = c;  M23 = s; M24 = 0;
		M31 = 0; M32 = -s; M33 = c; M34 = 0;
		M41 = 0; M42 = 0;  M43 = 0; M44 = 1;
	}
	public void SetRotationY(float angle)
	{
		float s = MathF.Sin(angle);
		float c = MathF.Cos(angle);

		M11 = c; M12 = 0; M13 = -s; M14 = 0;
		M21 = 0; M22 = 1; M23 = 0;  M24 = 0;
		M31 = s; M32 = 0; M33 = c;  M34 = 0;
		M41 = 0; M42 = 0; M43 = 0;  M44 = 1;
	}
	public void SetRotationZ(float angle)
	{
		float s = MathF.Sin(angle);
		float c = MathF.Cos(angle);

		M11 = c;  M12 = s; M13 = 0; M14 = 0;
		M21 = -s; M22 = c; M23 = 0; M24 = 0;
		M31 = 0;  M32 = 0; M33 = 1; M34 = 0;
		M41 = 0;  M42 = 0; M43 = 0; M44 = 1;
	}

	public void SetRotation(float pitch, float yaw, float roll)
	{
		float sp = MathF.Sin(pitch);
		float cp = MathF.Cos(pitch);

		float sy = MathF.Sin(yaw);
		float cy = MathF.Cos(yaw);

		float sr = MathF.Sin(roll);
		float cr = MathF.Cos(roll);
		
		M11 = cr *cy + sr *sp *sy;
		M12 = sr *cp;
		M13 = -cr *sy + sr *sp *cy;
		M14 = 0;
		
		M21 = -sr *cy + cr *sp *sy;
		M22 = cr *cp;
		M23 = sr *sy + cr *sp *cy;
		M24 = 0;
		
		M31 = cp *sy;
		M32 = -sp;
		M33 = cp *cy;
		M34 = 0;
		
		M41 = 0;
		M42 = 0;
		M43 = 0;
		M44 = 1;
	}

	public void SetRotation(Vector3Float euler)
	{
		SetRotation(euler.X, euler.Y, euler.Z);
	}

	public void SetRotation(Point3Float origin, Vector3Float direction, float angle)
	{
		Matrix4x4Float translateBack = CreateTranslationMatrix(-origin.Vector3);

		float s = MathF.Sin(angle);
		float c = MathF.Cos(angle);
		
		float c1 = 1 - c;
		Matrix4x4Float rotate = new()
		                     {
			                     M11 = c + direction.X *direction.X *c1,
			                     M12 = direction.X     *direction.Y *c1 - direction.Z *s,
			                     M13 = direction.X     *direction.Z *c1 + direction.Y *s,
			                     M14 = 0,
			 
			                     M21 = direction.Y *direction.X *c1 + direction.Z *s,
			                     M22 = c                            +direction.Y  *direction.Y *c1,
			                     M23 = direction.Y                                *direction.Z *c1 - direction.X *s,
			                     M24 = 0,
			 
			                     M31 = direction.Z *direction.X *c1 - direction.Y *s,
			                     M32 = direction.Z *direction.Y *c1 + direction.X *s,
			                     M33 = c                            + direction.Z *direction.Z *c1,
			                     M34 = 0,
			 
			                     M41 = 0,
			                     M42 = 0,
			                     M43 = 0,
			                     M44 = 1
		                     };

		Matrix4x4Float translateForward = CreateTranslationMatrix(origin.Vector3);
		this = Mul(Mul(translateBack, rotate), translateForward);
	}
	#endregion

	#region Transposition and inversion
	public void Transpose(out Matrix4x4Float result)
	{
		result = new Matrix4x4Float
		         {
			         M11 = this.M11, M21 = this.M12, M31 = this.M13, M41 = this.M14,
			         M12 = this.M21, M22 = this.M22, M32 = this.M23, M42 = this.M24,
			         M13 = this.M31, M23 = this.M32, M33 = this.M33, M43 = this.M34,
			         M14 = this.M41, M24 = this.M42, M34 = this.M43, M44 = this.M44,
		         };
	}

	public bool Invert(out Matrix4x4Float result)
	{
		float a, b, c, d, e, f;

		a = M33 * M44 - M43 * M34;
		b = M23 * M44 - M43 * M24;
		c = M23 * M34 - M33 * M24;
		d = M13 * M44 - M43 * M14;
		e = M13 * M34 - M33 * M14;
		f = M13 * M24 - M23 * M14;

		float m11 = M22  * a - M32 * b + M42 * c;
		float m12 = -M12 * a + M32 * d - M42 * e;
		float m13 = M12  * b - M22 * d + M42 * f;
		float m14 = -M12 * c + M22 * e - M32 * f;

		float m21 = -M21 * a + M31 * b - M41 * c;
		float m22 = M11  * a - M31 * d + M41 * e;
		float m23 = -M11 * b + M21 * d - M41 * f;
		float m24 = M11  * c - M21 * e + M31 * f;

		a = M31 * M42 - M41 * M32;
		b = M21 * M42 - M41 * M22;
		c = M21 * M32 - M31 * M22;
		d = M11 * M42 - M41 * M12;
		e = M11 * M32 - M31 * M12;
		f = M11 * M22 - M21 * M12;

		float m31 = M24  * a - M34 * b + M44 * c;
		float m32 = -M14 * a + M34 * d - M44 * e;
		float m33 = M14  * b - M24 * d + M44 * f;
		float m34 = -M14 * c + M24 * e - M34 * f;

		float m41 = -M23 * a + M33 * b - M43 * c;
		float m42 = M13  * a - M33 * d + M43 * e;
		float m43 = -M13 * b + M23 * d - M43 * f;
		float m44 = M13  * c - M23 * e + M33 * f;

		float div = M11 * m11 + M21 * m12 + M31 * m13 + M41 * m14;
		if (div > -0.0001f && div < 0.0001f)
		{
			result = Identity;
			return false;
		}

		d = 1.0f / div;
		result = new Matrix4x4Float
		         {
			         M11 = m11 * d, M12 = m12 * d, M13 = m13 * d, M14 = m14 * d,
			         M21 = m21 * d, M22 = m22 * d, M23 = m23 * d, M24 = m24 * d,
			         M31 = m31 * d, M32 = m32 * d, M33 = m33 * d, M34 = m34 * d,
			         M41 = m41 * d, M42 = m42 * d, M43 = m43 * d, M44 = m44 * d,
		         };
		return true;
	}
	#endregion
}