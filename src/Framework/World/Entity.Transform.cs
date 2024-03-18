using Veldrid;

namespace Mine.Framework;

// Transform part of entity:
// - local position, rotation and scale are always up-to-date
// - world position and rotation are evaluated at access
// - matrices are evaluated at access
public sealed partial class Entity
{
	[Flags]
	private enum TransformFlags
	{
		None                    = 0,
		LocalToWorldMatrixDirty = 0x01,
		WorldToLocalMatrixDirty = 0x02,
		MatricesDirty = LocalToWorldMatrixDirty | WorldToLocalMatrixDirty,
		AllDirty = MatricesDirty
	}

	private TransformFlags _transformFlags          = TransformFlags.AllDirty;

	// local coordinates in parent space
	private Point3Float _localPosition = Point3Float.Zero;
	public Point3Float LocalPosition
	{
		get => _localPosition;
		set
		{
			_localPosition   =  value;
			PropagateFlagToChildren(TransformFlags.MatricesDirty);
		}
	}

	private QuaternionFloat _localRotation = QuaternionFloat.Identity;
	public QuaternionFloat LocalRotation
	{
		get => _localRotation;
		set
		{
			_localRotation   =  value;
			PropagateFlagToChildren(TransformFlags.MatricesDirty);
		}
	}

	private Vector3Float _localScale = Vector3Float.One;
	public Vector3Float LocalScale
	{
		get => _localScale;
		set
		{
			_localScale      =  value;
			PropagateFlagToChildren(TransformFlags.MatricesDirty);
		}
	}

	// world space coordinates
	public Point3Float     WorldPosition => Parent is null ? LocalPosition : LocalPosition.Transform(Parent.LocalToWorldMatrix);
	public QuaternionFloat WorldRotation => Parent is null ? -LocalRotation : QuaternionFloat.CreateFromRotationMatrix(LocalToWorldMatrix);
	
	// transform coordinates that are local to this entity to world space
	public ulong          LocatToWorldMatrixUpdatedAt = 0;
	private Matrix4x4Float _localToWorldMatrix      = Matrix4x4Float.Identity;
	public  Matrix4x4Float LocalToWorldMatrix
	{
		get
		{
			if ((_transformFlags & TransformFlags.LocalToWorldMatrixDirty) != 0)
			{
				Console.WriteLine($"Updating matrix, parent is null: {Parent is null}");
				Matrix4x4Float translate = Matrix4x4Float.CreateTranslationMatrix(LocalPosition.Vector3);
				Matrix4x4Float rotate    = Matrix4x4Float.CreateRotationMatrix(LocalRotation);
				Matrix4x4Float scale     = Matrix4x4Float.CreateScaleMatrix(LocalScale);

				Matrix4x4Float local = Matrix4x4Float.Mul(Matrix4x4Float.Mul(scale, rotate), translate);
				_localToWorldMatrix          =  Parent is null ? local : Matrix4x4Float.Mul(local, Parent.LocalToWorldMatrix);
				_transformFlags              &= ~TransformFlags.LocalToWorldMatrixDirty;
				LocatToWorldMatrixUpdatedAt =  Engine.Timing.UpdateFrame;
			}
			return _localToWorldMatrix;
		}
		private set => _localToWorldMatrix = value;
	}

	private Matrix4x4Float _worldToLocalMatrix = Matrix4x4Float.Identity;
	public  Matrix4x4Float WorldToLocalMatrix
	{
		get
		{
			if ((_transformFlags & TransformFlags.WorldToLocalMatrixDirty) != 0)
			{
				if (LocalToWorldMatrix.Invert(out Matrix4x4Float invertedMatrix))
				{
					_worldToLocalMatrix = invertedMatrix;
				}
				_transformFlags &= ~TransformFlags.WorldToLocalMatrixDirty;
			}

			return _worldToLocalMatrix;
		}
		private set => _worldToLocalMatrix = value;
	}
	
	// resource for shader
	private readonly ShaderResourceSet _shaderResourceWorldMatrix;

	public ShaderResourceSet ShaderResourceWorldMatrix
	{
		get
		{
			_shaderResourceWorldMatrix.Update();
			return _shaderResourceWorldMatrix;
		}
	}

	#region Helpers
	private void PropagateFlagToChildren(TransformFlags flag)
	{
		_transformFlags |= flag;
		for (int i = 0; i < Children.Count; ++i)
		{
			Children[i].PropagateFlagToChildren(flag);
		}
	}
	#endregion
}