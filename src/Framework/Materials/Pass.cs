using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace Mine.Framework;

public sealed class Pass : IDisposable
{
	public readonly string                       Name;
	public readonly int                          Order;
	public readonly BlendStateDescription        BlendStateDescription;
	public readonly DepthStencilStateDescription DepthStencilStateDescription;
	public readonly RasterizerStateDescription   RasterizerStateDescription;
	public readonly MaterialShader               VertexShader;
	public readonly MaterialShader               PixelShader;
	public readonly ShaderResourceSet[]          ShaderResourceSets;

	private Veldrid.Shader[]? Shaders;
	
	public Pass(
		string                       name,
		int                          order,
		BlendStateDescription        blendStateDescription,
		DepthStencilStateDescription depthStencilStateDescription,
		RasterizerStateDescription   rasterizerStateDescription,
		MaterialShader               vertexShader,
		MaterialShader               pixelShader,
		ShaderResourceSet[]          shaderResourceSets
	)
	{
		Name                         = name;
		Order                        = order;
		BlendStateDescription        = blendStateDescription;
		DepthStencilStateDescription = depthStencilStateDescription;
		RasterizerStateDescription   = rasterizerStateDescription;
		VertexShader                 = vertexShader;
		PixelShader                  = pixelShader;
		ShaderResourceSets           = shaderResourceSets;
	}

	public void SetupDrawing(CommandList commandList)
	{
		
	}

	private void CreateShaders()
	{
		Shaders = Engine.Graphics.Factory.CreateFromSpirv(VertexShader.CreateDescription(), PixelShader.CreateDescription());
	}

	public void Dispose()
	{
		if (Shaders != null)
		{
			foreach (var shader in Shaders)
			{
				shader?.Dispose();
			}

			Shaders = null;
		}

		for (int i = 0; i < ShaderResourceSets.Length; ++i)
		{
			ShaderResourceSets[i].Dispose();
		}
	}
}