using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceTextureReadOnly : ShaderResource
{
	public readonly AssetReference Reference;
	
	public ShaderResourceTextureReadOnly(string name, ShaderStages stages, AssetReference reference) : base(name, ResourceKind.TextureReadOnly, stages, ShaderResourceStorage.Material)
	{
		Reference = reference;
	}
}