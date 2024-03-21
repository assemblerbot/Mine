namespace Mine.Framework;

public class CameraComponent : RendererComponent
{
	private   bool                                  _shaderResourceDirty             = true;
	private ShaderResourceSetViewProjectionMatrix _shaderResourceSetViewProjection = new();
	
	public CameraComponent(int renderOrder, ulong renderMask, Clipper clipper) : base(renderOrder, renderMask, clipper)
	{
	}

	public override void AfterEntityFlagsChanged(EntityFlags oldFlags, EntityFlags newFlags)
	{
		if (newFlags.HasFlag(EntityFlags.LocalToWorldMatrixDirty))
		{
			_shaderResourceDirty = true;
		}
	}

	public override void Dispose()
	{
		base.Dispose();

		_shaderResourceSetViewProjection.Dispose();
	}

	public override ShaderResourceSetViewProjectionMatrix GetShaderResourceSetViewProjectionMatrix()
	{
		if (!_shaderResourceDirty)
		{
			return _shaderResourceSetViewProjection;
		}

		// TODO
		Matrix4x4Float view       = Matrix4x4Float.CreateViewLookAtLH(new Point3Float(0, 0, -10), new Point3Float(0, 0, 0), Vector3Float.PlusY);
		Matrix4x4Float projection = Matrix4x4Float.CreateProjectionPerspectiveLH(Single.Pi / 4f, (float)Engine.Window.Size.X / Engine.Window.Size.Y, 0.1f, 100f);

		_shaderResourceSetViewProjection.Set(Matrix4x4Float.Mul(view, projection));
		_shaderResourceDirty = false;
		return _shaderResourceSetViewProjection;
	}
}