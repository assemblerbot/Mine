using System.Runtime.InteropServices;

namespace Mine.Framework;

[Serializable, StructLayout(LayoutKind.Explicit)]
public record struct QuaternionFloat
{
	public const int SizeInBytes = 4 * sizeof(float);
	
	[FieldOffset(sizeof(float) * 0)] public float X;
	[FieldOffset(sizeof(float) * 1)] public float Y;
	[FieldOffset(sizeof(float) * 2)] public float Z;
	[FieldOffset(sizeof(float) * 3)] public float W;

	public static QuaternionFloat Identity = new (0,0,0,1);
	
	#region Creation
	public QuaternionFloat(float x, float y, float z, float w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}
	
	public static QuaternionFloat CreateFromRotationMatrix(Matrix4x4Float matrix)
	{
		float trace = matrix.M11 + matrix.M22 + matrix.M33;

		QuaternionFloat q = new QuaternionFloat();

		if (trace > 0.0f)
		{
			float s = (float)Math.Sqrt(trace + 1.0f);
			q.W = s                         * 0.5f;
			s   = 0.5f                      / s;
			q.X = (matrix.M23 - matrix.M32) * s;
			q.Y = (matrix.M31 - matrix.M13) * s;
			q.Z = (matrix.M12 - matrix.M21) * s;
		}
		else
		{
			if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
			{
				float s    = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
				float invS = 0.5f / s;
				q.X = 0.5f                      * s;
				q.Y = (matrix.M12 + matrix.M21) * invS;
				q.Z = (matrix.M13 + matrix.M31) * invS;
				q.W = (matrix.M23 - matrix.M32) * invS;
			}
			else if (matrix.M22 > matrix.M33)
			{
				float s    = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
				float invS = 0.5f / s;
				q.X = (matrix.M21 + matrix.M12) * invS;
				q.Y = 0.5f                      * s;
				q.Z = (matrix.M32 + matrix.M23) * invS;
				q.W = (matrix.M31 - matrix.M13) * invS;
			}
			else
			{
				float s    = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
				float invS = 0.5f / s;
				q.X = (matrix.M31 + matrix.M13) * invS;
				q.Y = (matrix.M32 + matrix.M23) * invS;
				q.Z = 0.5f                      * s;
				q.W = (matrix.M12 - matrix.M21) * invS;
			}
		}

		return q;
	}
	#endregion

	#region Operators
	public static QuaternionFloat operator -(QuaternionFloat value)
	{
		QuaternionFloat result;

		result.X = -value.X;
		result.Y = -value.Y;
		result.Z = -value.Z;
		result.W = -value.W;

		return result;
	}
	#endregion
	
	
	public QuaternionFloat Conjugate()
	{
		QuaternionFloat ans;

		ans.X = -X;
		ans.Y = -Y;
		ans.Z = -Z;
		ans.W = W;

		return ans;
	}

	public QuaternionFloat Inversed()
	{
		//  -1   (       a              -v       )
		// q   = ( -------------   ------------- )
		//       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

		QuaternionFloat ans;

		float ls      = X * X + Y * Y + Z * Z + W * W;
		float invNorm = 1.0f / ls;

		ans.X = -X * invNorm;
		ans.Y = -Y * invNorm;
		ans.Z = -Z * invNorm;
		ans.W = W  * invNorm;

		return ans;
	}	
}