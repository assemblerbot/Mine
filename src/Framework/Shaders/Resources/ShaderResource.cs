using Veldrid;

namespace Mine.Framework;

public abstract class ShaderResource
{
	public readonly string       Name;
	public readonly ResourceKind Kind;

	protected ShaderResource(string name, ResourceKind kind)
	{
		Name   = name;
		Kind   = kind;
	}

	public ResourceLayoutElementDescription CreateResourceLayoutElementDescription(ShaderStages stages)
	{
		return new ResourceLayoutElementDescription(Name, Kind, stages);
	}

	public abstract BindableResource GetOrCreateBindableResource();
	public abstract void             Dispose();
}