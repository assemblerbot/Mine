using Veldrid;

namespace Mine.Framework;

public sealed class Pass
{
	public readonly string                       Name;
	public readonly BlendStateDescription        BlendStateDescription;
	public readonly DepthStencilStateDescription DepthStencilStateDescription;
	public readonly RasterizerStateDescription   RasterizerStateDescription;
	public readonly AssetReference               VertexShader;
	public readonly AssetReference               PixelShader;
	public readonly ShaderResourceSet[]          ShaderResourceSets;

	public Pass(
		string                       name,
		BlendStateDescription        blendStateDescription,
		DepthStencilStateDescription depthStencilStateDescription,
		RasterizerStateDescription   rasterizerStateDescription,
		AssetReference               vertexShader,
		AssetReference               pixelShader,
		ShaderResourceSet[]          shaderResourceSets
	)
	{
		Name                         = name;
		BlendStateDescription        = blendStateDescription;
		DepthStencilStateDescription = depthStencilStateDescription;
		RasterizerStateDescription   = rasterizerStateDescription;
		VertexShader                 = vertexShader;
		PixelShader                  = pixelShader;
		ShaderResourceSets           = shaderResourceSets;
	}
}