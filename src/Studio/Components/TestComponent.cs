using Mine.Framework;
using Veldrid;

namespace Mine.Studio;

public sealed class TestComponent : Component, IUpdatable
{
	public int UpdateOrder => 0;

	//public static SceneReference TestAsset = new(@"Test/suzanne.fbx.scene");
	public static SceneReference TestAsset = new(@"Test/cube.fbx.scene");

	public static AssetReference VertexShaderAsset = new(@"Test/test.vsh.spirv");
	public static AssetReference PixelShaderAsset  = new(@"Test/test.psh.spirv");
	
	private class TestMaterial : Material
	{
		// dynamic access
		public Vector4Float    LightDirection { set => SetShaderConstant("Main", ShaderResourceSetKind.MaterialProperties0, nameof(LightDirection), value); }
		public Color4FloatRGBA SurfaceColor   { set => SetShaderConstant("Main", ShaderResourceSetKind.MaterialProperties1, nameof(SurfaceColor),   value); }

		private static readonly ulong _mainPassId = Engine.NextUniqueId; // TODO - I don't like it
		
		public TestMaterial(
			AssetReference mainTexture
		)
			: base(
				new Pass(
					_mainPassId,
					"Main",
					order:100,
					BlendStateDescription.SingleOverrideBlend,
					new DepthStencilStateDescription(
						depthTestEnabled:true,
						depthWriteEnabled:true,
						ComparisonKind.LessEqual
					),
					new RasterizerStateDescription(
						FaceCullMode.Back,
						PolygonFillMode.Solid,
						FrontFace.Clockwise,
						depthClipEnabled:true,
						scissorTestEnabled:false
					),

					new MaterialShader(VertexShaderAsset, "main"),
					new MaterialShader(PixelShaderAsset,  "main"),
					
					new[] {
						      (ShaderResourceSetKind.WorldMatrix,          ShaderStages.Vertex),
						      (ShaderResourceSetKind.ViewProjectionMatrix, ShaderStages.Vertex),
						      (ShaderResourceSetKind.MaterialProperties0,  ShaderStages.Vertex),
						      (ShaderResourceSetKind.MaterialProperties1,  ShaderStages.Fragment),
						      (ShaderResourceSetKind.AmbientLight,         ShaderStages.Fragment),
						      (ShaderResourceSetKind.DirectionalLight,     ShaderStages.Fragment),
					      },
					
					new[] {
						      new PassConstBuffer(
							      "MaterialVertex",
							      ShaderResourceSetKind.MaterialProperties0,
							      new PassConstBufferVector4Float(nameof(LightDirection), new Vector4Float(1, -1, 1, 0).Normalized())
						      ),
					
						      new PassConstBuffer(
							      "MaterialPixel",
							      ShaderResourceSetKind.MaterialProperties1,
							      new PassConstBufferColor4FloatRgba(nameof(SurfaceColor), new Color4FloatRGBA(0f, 0.5f, 1.0f, 1f))
						      )
					      }
				)
			)
		{}
	}

	private TestMaterial _material;
	private Entity?      _testEntity;
	private float        _time = 0;
	
	public override void AfterAddedToWorld()
	{
		_material   = new TestMaterial(null!);
		_testEntity = InstantiateTestPrefab(Entity, _material);
		CreateCamera();
		CreateLights();
		Engine.World.RegisterUpdatable(this);
	}

	public override void BeforeRemovedFromWorld()
	{
		Engine.World.UnregisterUpdatable(this);
		Entity.DestroyChildren();
	}

	public static Entity InstantiateTestPrefab(Entity parent, Material material)
	{
		SceneReference asset    = TestAsset;
		Entity?        instance = PrefabUtility.Instantiate(asset);
		if (instance is null)
		{
			throw new NullReferenceException("Asset with id 'ea7581d6-9b32-4a83-bf7a-ef904aabafc7' is missing!");
		}
		instance.GetChild(0)!
			.AddComponent(new MeshComponent(asset, 0, material, ulong.MaxValue)) // mesh:Suzanne material:DefaultMaterial
			;

		parent.AddChild(instance);
		return instance;
	}

	public void CreateCamera()
	{
		Entity entity = new Entity().AddComponent(
			new CameraComponent(
				0,
				ulong.MaxValue,
				new MaskClipper(),
				new List<string>{"Main"}
			)
		).Entity;
		
		entity.LocalPosition = new Point3Float(0, 0, -3);
		Engine.World.Root.AddChild(entity);
	}

	private void CreateLights()
	{
		// ambient
		{
			AmbientLightComponent light = new (ulong.MaxValue);
			Entity                entity       = new Entity().AddComponent(light).Entity;
			Engine.World.Root.AddChild(entity);

			light.Color = new Color4FloatRGBA(0.1f, 0.1f, 0.1f, 0f);
		}
		
		// directional
		{
			DirectionalLightComponent light  = new (UInt64.MaxValue);
			Entity                    entity = new Entity().AddComponent(light).Entity;
			Engine.World.Root.AddChild(entity);

			light.Color          = new Color4FloatRGBA(1.0f, 1.0f, 0.8f, 1f);
			entity.LocalRotation = QuaternionFloat.CreateFromYawPitchRoll(0.7f, 0.7f, 0.7f);
		}
	}

	public void Update(double timeDelta)
	{
		_time += (float)timeDelta;
		
		if (_testEntity is not null)
		{
			const float speed = 0.3f;
			_testEntity.LocalRotation = QuaternionFloat.CreateFromYawPitchRoll(_time * speed, _time * 0.3f * speed, _time * 0.1f * speed);
			//_testEntity.LocalPosition = new Point3Float(0, 0, _time);
			//_testEntity.LocalScale    = new Vector3Float(0.01f, 0.01f, 0.01f);
			_material.SurfaceColor = new Color4FloatRGBA(MathF.Sin(_time) * 0.5f + 0.5f, 0.5f, 1f, 1f);
		}
	}
}