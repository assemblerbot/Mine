namespace Mine.Framework;

public sealed class ShaderResourceSet
{
	public readonly ShaderResource[] Resources;

	public ShaderResourceSet(params ShaderResource[] resources)
	{
		Resources = resources;
	}
}