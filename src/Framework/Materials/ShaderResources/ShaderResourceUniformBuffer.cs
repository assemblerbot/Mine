using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceUniformBuffer : ShaderResource
{
	public readonly ShaderConstant[] Constants;
	
	public ShaderResourceUniformBuffer(string name, ShaderStages stages, ShaderResourceStorage storage, params ShaderConstant[] constants) : base(name, ResourceKind.UniformBuffer, stages, storage)
	{
		Constants = constants;
	}
}