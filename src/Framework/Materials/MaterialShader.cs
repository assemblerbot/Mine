using Veldrid;

namespace Mine.Framework;

public sealed class MaterialShader
{
	public readonly AssetReference Asset;
	public readonly string         EntryPoint;
	public          ShaderStages   Stages { get; internal set; }

	public MaterialShader(AssetReference asset, string entryPoint)
	{
		Asset      = asset;
		EntryPoint = entryPoint;
	}

	public ShaderDescription CreateDescription()
	{
		return new ShaderDescription(Stages, Asset.Value, EntryPoint);
	}
}