using Veldrid;

namespace Mine.Framework;

public sealed class MaterialShader
{
	public readonly AssetReference Asset;
	public readonly ShaderStages   Stages;
	public readonly string         EntryPoint;

	public MaterialShader(AssetReference asset, ShaderStages stages, string entryPoint)
	{
		Asset      = asset;
		Stages     = stages;
		EntryPoint = entryPoint;
	}

	public ShaderDescription CreateDescription()
	{
		return new ShaderDescription(Stages, Asset.Value, EntryPoint);
	}
}