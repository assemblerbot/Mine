using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSampler : ShaderResource
{
	public readonly SamplerDescription SamplerDescription;

	private Sampler? _sampler;
	
	public ShaderResourceSampler(string name, ShaderStages stages, SamplerDescription samplerDescription) : base(name, ResourceKind.Sampler, stages, ShaderResourceStorage.Material)
	{
		SamplerDescription = samplerDescription;
	}

	public override BindableResource GetOrCreateBindableResource()
	{
		if (_sampler is not null)
		{
			return _sampler;
		}

		_sampler = Engine.Graphics.Factory.CreateSampler(SamplerDescription);
		return _sampler;
	}

	public override void             Dispose()
	{
		_sampler?.Dispose();
		_sampler = null;
	}
}