using Veldrid;

namespace Mine.Framework;

// Transform part of entity:
// - local position, rotation and scale are always up-to-date
// - world position and rotation are evaluated at access
// - matrices are evaluated at access
public sealed partial class Entity
{
	// local coordinates in parent space
	private Point3Float _localPosition = Point3Float.Zero;
	public Point3Float LocalPosition
	{
		get => _localPosition;
		set
		{
			_localPosition = value;
			PropagateFlagsToChildren(EntityFlags.MatricesDirty);
		}
	}

	private QuaternionFloat _localRotation = QuaternionFloat.Identity;
	public QuaternionFloat LocalRotation
	{
		get => _localRotation;
		set
		{
			_localRotation = value;
			PropagateFlagsToChildren(EntityFlags.MatricesDirty);
		}
	}

	private Vector3Float _localScale = Vector3Float.One;
	public Vector3Float LocalScale
	{
		get => _localScale;
		set
		{
			_localScale = value;
			PropagateFlagsToChildren(EntityFlags.MatricesDirty);
		}
	}

	// world space coordinates
	public Point3Float     WorldPosition => Parent is null ? LocalPosition : LocalPosition.Transform(Parent.LocalToWorldMatrix);
	public QuaternionFloat WorldRotation => Parent is null ? -LocalRotation : QuaternionFloat.CreateFromRotationMatrix(LocalToWorldMatrix);

	public Vector3Float Forward => Parent is null ? Vector3Float.PlusZ : Vector3Float.PlusZ.Transform(Parent.LocalToWorldMatrix);
	public Vector3Float Up      => Parent is null ? Vector3Float.PlusY : Vector3Float.PlusY.Transform(Parent.LocalToWorldMatrix);
	
	// transform coordinates that are local to this entity to world space
	private ulong          _localToWorldMatrixUpdatedAt = 0;
	private Matrix4x4Float _localToWorldMatrix          = Matrix4x4Float.Identity;
	public Matrix4x4Float LocalToWorldMatrix
	{
		get
		{
			if (IsFlags(EntityFlags.LocalToWorldMatrixDirty))
			{
				//Console.WriteLine($"Updating matrix, parent is null: {Parent is null}");
				Matrix4x4Float translate = Matrix4x4Float.CreateTranslationMatrix(LocalPosition.Vector3);
				Matrix4x4Float rotate    = Matrix4x4Float.CreateRotationMatrix(LocalRotation);
				Matrix4x4Float scale     = Matrix4x4Float.CreateScaleMatrix(LocalScale);

				Matrix4x4Float local = Matrix4x4Float.Mul(Matrix4x4Float.Mul(scale, rotate), translate);
				_localToWorldMatrix = Parent is null ? local : Matrix4x4Float.Mul(local, Parent.LocalToWorldMatrix);
				ClearFlags(EntityFlags.LocalToWorldMatrixDirty);
				_localToWorldMatrixUpdatedAt = Engine.Timing.UpdateFrame;
			}
			return _localToWorldMatrix;
		}
		private set => _localToWorldMatrix = value;
	}

	private Matrix4x4Float _worldToLocalMatrix = Matrix4x4Float.Identity;
	public Matrix4x4Float WorldToLocalMatrix
	{
		get
		{
			if (IsFlags(EntityFlags.WorldToLocalMatrixDirty))
			{
				if (LocalToWorldMatrix.Invert(out Matrix4x4Float invertedMatrix))
				{
					_worldToLocalMatrix = invertedMatrix;
				}
				ClearFlags(EntityFlags.WorldToLocalMatrixDirty);
			}

			return _worldToLocalMatrix;
		}
		private set => _worldToLocalMatrix = value;
	}
	
	// resource for shader
	private ShaderResourceSetWorldMatrix? _shaderResourceWorldMatrix;
	public  ShaderResourceSetWorldMatrix ShaderResourceWorldMatrix
	{
		get
		{
			if (IsFlags(EntityFlags.ShaderResourceWorldMatrixDirty))
			{
				_shaderResourceWorldMatrix ??= new ShaderResourceSetWorldMatrix();
				_shaderResourceWorldMatrix.Set(LocalToWorldMatrix);
				ClearFlags(EntityFlags.ShaderResourceWorldMatrixDirty);
			}

			return _shaderResourceWorldMatrix!;
		}
	}
}