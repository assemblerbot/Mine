namespace Mine.Framework;

public sealed class ShadingPass
{
	public readonly string         Name;
	public readonly AssetReference VertexShader;
	public readonly AssetReference PixelShader;

	public ShadingPass(string name, AssetReference vertexShader, AssetReference pixelShader)
	{
		Name         = name;
		VertexShader = vertexShader;
		PixelShader  = pixelShader;
	}
}