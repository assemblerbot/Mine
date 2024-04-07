using Gfx;

namespace Mine.Framework;

public sealed class SharedPipeline : IDisposable
{
	public readonly Pipeline Pipeline;

	public SharedPipeline(GraphicsPipelineDescription description)
	{
		Pipeline = Engine.Graphics.Factory.CreateGraphicsPipeline(description);
	}

	public SharedPipeline(SharedMesh mesh, Pass pass, OutputDescription output)
	{
		GraphicsPipelineDescription description = new()
		                                          {
			                                          BlendState        = pass.BlendStateDescription,
			                                          DepthStencilState = pass.DepthStencilStateDescription,
			                                          RasterizerState   = pass.RasterizerStateDescription,
			                                          PrimitiveTopology = mesh.Topology,
			                                          ResourceLayouts   = pass.ResourceLayouts,
			                                          ShaderSet = new(
				                                          new[] { mesh.SharedVertexLayout.VertexLayoutDescription },
				                                          pass.Shaders
			                                          ),
			                                          Outputs = output
		                                          };

		Pipeline = Engine.Graphics.Factory.CreateGraphicsPipeline(description);
	}

	public void Dispose()
	{
		Pipeline.Dispose();
	}
}