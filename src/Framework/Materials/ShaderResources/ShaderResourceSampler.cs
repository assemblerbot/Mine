using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSampler : ShaderResource
{
	public readonly SamplerDescription SamplerDescription;
	
	public ShaderResourceSampler(string name, ShaderStages stages, SamplerDescription samplerDescription) : base(name, ResourceKind.Sampler, stages, ShaderResourceStorage.Material)
	{
		SamplerDescription = samplerDescription;
	}

	public override BindableResource GetOrCreateBindableResource()
	{
		throw new NotImplementedException();
	}

	public override void             Dispose()
	{
		throw new NotImplementedException();
	}
}