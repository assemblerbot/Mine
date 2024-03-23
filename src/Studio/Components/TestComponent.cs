using Mine.Framework;
using Veldrid;

namespace Mine.Studio;

public sealed class TestComponent : Component, IUpdatable
{
	public int UpdateOrder => 0;

	//public static SceneReference TestAsset = new(@"Test/suzanne.fbx.scene");
	public static SceneReference TestAsset = new(@"Test/cube.fbx.scene");

	public static AssetReference VertexShaderAsset = new(@"Test/test.vsh.spirv");
	public static AssetReference PixelShaderAsset = new(@"Test/test.psh.spirv");
	
	private class TestMaterial : Material
	{
		// dynamic access
		public Vector4Float    LightDirection { set => SetShaderConstant("Main", ShaderStages.Vertex,   "LightDirection", value); }
		public Color4FloatRGBA SurfaceColor   { set => SetShaderConstant("Main", ShaderStages.Fragment, "SurfaceColor",   value); }

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

					new MaterialShader(VertexShaderAsset, ShaderStages.Vertex,   "main"),
					new MaterialShader(PixelShaderAsset,  ShaderStages.Fragment, "main"),
					
					new[] {
						      ShaderResourceSetKind.WorldMatrix,
						      ShaderResourceSetKind.ViewProjectionMatrix,
						      ShaderResourceSetKind.VertexMaterialProperties,
						      ShaderResourceSetKind.PixelMaterialProperties,
					      },
					
					new PassShaderConstBuffer(
						"MaterialVertex",
						new PassShaderConstBufferVector4Float("LightDirection", new Vector4Float(1,1,1,0).Normalized())
					),
					
					new PassShaderConstBuffer(
						"MaterialPixel",
						new PassShaderConstBufferColor4FloatRGBA("SurfaceColor", new Color4FloatRGBA(0f,0.5f,1.0f,1f))
					)
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
		Entity entity = new Entity().AddComponent(new CameraComponent(0, ulong.MaxValue, new MaskClipper(), new List<string>{"Main"})).Entity;
		entity.LocalPosition = new Point3Float(0, 0, -3);
		Engine.World.Root.AddChild(entity);
	}

	public void Update(double timeDelta)
	{
		_time += (float)timeDelta;
		
		if (_testEntity is not null)
		{
			_testEntity.LocalRotation = QuaternionFloat.CreateFromYawPitchRoll(_time, _time * 0.3f, _time * 0.1f);
			//_testEntity.LocalPosition = new Point3Float(0, 0, _time);
			//_testEntity.LocalScale    = new Vector3Float(0.01f, 0.01f, 0.01f);
			_material.SurfaceColor = new Color4FloatRGBA(MathF.Sin(_time) * 0.5f + 0.5f, 0.5f, 1f, 1f);
		}
	}
}