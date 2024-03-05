using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceUniformBuffer : ShaderResource
{
	public readonly ShaderConstant[] Constants;
	
	public ShaderResourceUniformBuffer(string name, ShaderStages stages, params ShaderConstant[] constants) : base(name, ResourceKind.UniformBuffer, stages)
	{
		Constants = constants;
	}
}