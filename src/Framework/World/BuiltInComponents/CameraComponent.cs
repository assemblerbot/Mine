namespace Mine.Framework;

public class CameraComponent : RendererComponent
{
	private float _verticalFOV       = Single.Pi * 0.5f;
	private float _customAspectRatio = -1f;
	private float _nearPlane         = 0.1f;
	private float _farPlane          = 100f;

	public float VerticalFOV
	{
		get => _verticalFOV;
		set
		{
			_shaderResourceDirty = _verticalFOV != value;
			_verticalFOV         = value;
		}
	}

	public float CustomAspectRatio
	{
		get => _customAspectRatio;
		set
		{
			_shaderResourceDirty = _customAspectRatio != value;
			_customAspectRatio   = value;
		}
	}

	public float NearPlane
	{
		get => _nearPlane;
		set
		{
			_shaderResourceDirty = _nearPlane != value;
			_nearPlane           = value;
		}
	}

	public float FarPlane
	{
		get => _farPlane;
		set
		{
			_shaderResourceDirty = _farPlane != value;
			_farPlane            = value;
		}
	}
	
	private          bool                                  _shaderResourceDirty             = true;
	private readonly ShaderResourceSetViewProjectionMatrix _shaderResourceSetViewProjection = new();

	private          ulong                         _shaderResourceSetAmbientLightUpdatedAt = 0;
	private readonly ShaderResourceSetAmbientLight _shaderResourceSetAmbientLight          = new();

	private          ulong                         _shaderResourceSetDirectionalLightUpdatedAt = 0;
	private readonly ShaderResourceSetDirectionalLight _shaderResourceSetDirectionalLight          = new();
	
	public CameraComponent(int renderOrder, ulong renderMask, Clipper clipper, List<string> passes) : base(renderOrder, renderMask, clipper, passes)
	{
	}

	public override void AfterEntityFlagsChanged(EntityFlags oldFlags, EntityFlags newFlags)
	{
		base.AfterEntityFlagsChanged(oldFlags, newFlags);
		if (newFlags.HasFlag(EntityFlags.LocalToWorldMatrixDirty))
		{
			_shaderResourceDirty = true;
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		_shaderResourceSetViewProjection.Dispose();
		_shaderResourceSetAmbientLight.Dispose();
		_shaderResourceSetDirectionalLight.Dispose();
	}

	#region Shader resources
	public override ShaderResourceSetViewProjectionMatrix GetShaderResourceSetViewProjectionMatrix()
	{
		if (!_shaderResourceDirty)
		{
			return _shaderResourceSetViewProjection;
		}
		_shaderResourceDirty = false;

		Matrix4x4Float view = Matrix4x4Float.CreateViewLookAtLH(
			Entity.WorldPosition,
			Entity.WorldPosition + Entity.Forward,
			Entity.Up
		);
		
		Matrix4x4Float projection = Matrix4x4Float.CreateProjectionPerspectiveLH(
			_verticalFOV,
			_customAspectRatio != -1 ? _customAspectRatio : (float)Engine.Window.Size.X / Engine.Window.Size.Y, // TODO - aspect ratio by render target
			_nearPlane,
			_farPlane
		);

		Matrix4x4Float viewProjection = Matrix4x4Float.Mul(view, projection);
		_shaderResourceSetViewProjection.Set(viewProjection);
		return _shaderResourceSetViewProjection;
	}

	public override ShaderResourceSetAmbientLight GetShaderResourceSetAmbientLight(List<LightComponent> lights)
	{
		if (_shaderResourceSetAmbientLightUpdatedAt == Engine.Timing.RenderFrame)
		{
			return _shaderResourceSetAmbientLight;
		}
		_shaderResourceSetAmbientLightUpdatedAt = Engine.Timing.RenderFrame;

		int index = lights.FindIndex(x => x is AmbientLightComponent);
		if (index == -1)
		{
			_shaderResourceSetAmbientLight.SetEmpty();
		}
		else
		{
			AmbientLightComponent ambientLight = (AmbientLightComponent)lights[index];
			_shaderResourceSetAmbientLight.Set(ambientLight.Color);
		}

		return _shaderResourceSetAmbientLight;
	}

	public override ShaderResourceSetDirectionalLight GetShaderResourceSetDirectionalLight(List<LightComponent> lights)
	{
		if (_shaderResourceSetDirectionalLightUpdatedAt == Engine.Timing.RenderFrame)
		{
			return _shaderResourceSetDirectionalLight;
		}
		_shaderResourceSetDirectionalLightUpdatedAt = Engine.Timing.RenderFrame;

		int index = lights.FindIndex(x => x is DirectionalLightComponent);
		if (index == -1)
		{
			_shaderResourceSetDirectionalLight.SetEmpty();
		}
		else
		{
			DirectionalLightComponent directionalLight = (DirectionalLightComponent)lights[index];
			_shaderResourceSetDirectionalLight.Set(directionalLight.Color, directionalLight.Entity.Forward);
		}

		return _shaderResourceSetDirectionalLight;
	}
	#endregion
}