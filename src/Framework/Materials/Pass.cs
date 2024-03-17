using Veldrid;
using Veldrid.SPIRV;

namespace Mine.Framework;

public sealed class Pass : IDisposable
{
	public readonly ulong                        Id;
	public readonly string                       Name;
	public readonly int                          Order;
	public readonly BlendStateDescription        BlendStateDescription;
	public readonly DepthStencilStateDescription DepthStencilStateDescription;
	public readonly RasterizerStateDescription   RasterizerStateDescription;
	public readonly MaterialShader               VertexShader;
	public readonly MaterialShader               PixelShader;
	public readonly ShaderResourceSet[]          ShaderResourceSets;
	
	public readonly ResourceLayout[]             ResourceLayouts;

	public Shader[]? Shaders;
	
	public Pass(
		ulong                        id,
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
		Id                           = id;
		Name                         = name;
		Order                        = order;
		BlendStateDescription        = blendStateDescription;
		DepthStencilStateDescription = depthStencilStateDescription;
		RasterizerStateDescription   = rasterizerStateDescription;
		VertexShader                 = vertexShader;
		PixelShader                  = pixelShader;
		ShaderResourceSets           = shaderResourceSets;

		ResourceLayouts = new ResourceLayout[shaderResourceSets.Length];
		for (int i = 0; i < shaderResourceSets.Length; ++i)
		{
			ResourceLayouts[i] = shaderResourceSets[i].ResourceLayout!;
		}
	}

	public void SetupDrawing(CommandList commandList)
	{
		for (int i = 0; i < ShaderResourceSets.Length; ++i)
		{
			commandList.SetGraphicsResourceSet((uint)i, ShaderResourceSets[i].ResourceSet!);
		}
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