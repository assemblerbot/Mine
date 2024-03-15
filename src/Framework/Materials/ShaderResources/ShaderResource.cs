using Veldrid;

namespace Mine.Framework;

public abstract class ShaderResource
{
	public readonly string       Name;
	public readonly ResourceKind Kind;
	public readonly ShaderStages Stages;

	protected ShaderResource(string name, ResourceKind kind, ShaderStages stages, ShaderResourceStorage storage)
	{
		Name   = name;
		Kind   = kind;
		Stages = stages;
	}

	public ResourceLayoutElementDescription CreateResourceLayoutElementDescription()
	{
		return new ResourceLayoutElementDescription(Name, Kind, Stages);
	}

	public abstract BindableResource GetOrCreateBindableResource();
	public abstract void             Dispose();
}