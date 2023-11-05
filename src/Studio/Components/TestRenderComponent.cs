using System.Numerics;
using System.Text;
using GameToolkit.Framework;
using Veldrid;
using Veldrid.SPIRV;

namespace GameToolkit.Studio.Components;

public class TestRenderComponent : Component, IUpdatable, IRenderable
{
	struct VertexPositionColor
	{
		public Vector2   Position; // This is the position, in normalized device coordinates.
		public RgbaFloat Color;    // This is the color of the vertex.

		public VertexPositionColor(Vector2 position, RgbaFloat color)
		{
			Position = position;
			Color    = color;
		}

		public const uint SizeInBytes = 24;
	}

	
	private DeviceBuffer _vertexBuffer;
	private DeviceBuffer _indexBuffer;
	private Shader[]     _shaders;
	private Pipeline     _pipeline;
	private CommandList  _commandList = null!;

	private const string VertexCode = @"
#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

void main()
{
    gl_Position = vec4(Position, 0, 1);
    fsin_Color = Color;
}";

	private const string FragmentCode = @"
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = fsin_Color;
}";

	public void Init()
	{
		Engine.Scene.RegisterUpdatable(this);
		Engine.Renderer.RegisterRenderable(this);

		ResourceFactory factory = Engine.Renderer.Factory;
		
		VertexPositionColor[] quadVertices =
		{
			new VertexPositionColor(new Vector2(-0.75f, 0.75f),  RgbaFloat.Red),
			new VertexPositionColor(new Vector2(0.75f,  0.75f),  RgbaFloat.Green),
			new VertexPositionColor(new Vector2(-0.75f, -0.75f), RgbaFloat.Blue),
			new VertexPositionColor(new Vector2(0.75f,  -0.75f), RgbaFloat.Yellow)
		};
		
ushort[] quadIndices = { 0, 1, 2, 3 };

            _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

            Engine.Renderer.Device.UpdateBuffer(_vertexBuffer, 0, quadVertices);
            Engine.Renderer.Device.UpdateBuffer(_indexBuffer,  0, quadIndices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(VertexCode),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(FragmentCode),
                "main");

            _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);

            pipelineDescription.Outputs = Engine.Renderer.Device.SwapchainFramebuffer.OutputDescription;
            _pipeline                   = factory.CreateGraphicsPipeline(pipelineDescription);

            _commandList                = factory.CreateCommandList();
	}

	public void Draw()
	{
		_commandList.Begin();
		_commandList.SetFramebuffer(Engine.Renderer.Device.SwapchainFramebuffer);
		_commandList.ClearColorTarget(0, RgbaFloat.Black);
		
		_commandList.SetVertexBuffer(0, _vertexBuffer);
		_commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
		_commandList.SetPipeline(_pipeline);
		_commandList.DrawIndexed(
			indexCount: 4,
			instanceCount: 1,
			indexStart: 0,
			vertexOffset: 0,
			instanceStart: 0);
		
		_commandList.End();
		Engine.Renderer.Device.SubmitCommands(_commandList);
	}
	
	public void Update(double timeDelta)
	{
		int x = 0;
		//Console.WriteLine($"TestRenderComponent.Update({timeDelta})");
	}

	public override void Dispose()
	{
		_vertexBuffer.Dispose();
		_indexBuffer.Dispose();
		
		foreach(Shader shader in _shaders)
		{
			shader.Dispose();
		}

		_pipeline.Dispose();
		
		_commandList.Dispose();
		_commandList = null!;
	}
}