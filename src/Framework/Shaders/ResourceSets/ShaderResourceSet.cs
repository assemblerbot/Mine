using Veldrid;

namespace Mine.Framework;

public class ShaderResourceSet : IDisposable
{
	// shader stage independent
	public readonly ShaderResource[] Resources;

	// shader stage dependent
	private readonly MultiStageResourceLayout _resourceLayout;
	private readonly MultiStageResourceSet    _resourceSet;

	public ShaderResourceSet(params ShaderResource[] resources)
	{
		Resources       = resources;
		
		_resourceLayout = new MultiStageResourceLayout(GetResourceLayoutDescription);
		_resourceSet    = new MultiStageResourceSet(GetResourceSetDescription);
	}

	public ResourceLayout GetResourceLayout(ShaderStages stages)
	{
		return _resourceLayout.Get(stages);
	}

	public ResourceSet GetResourceSet(ShaderStages stages)
	{
		return _resourceSet.Get(stages);
	}

	private ResourceLayoutDescription GetResourceLayoutDescription(ShaderStages stages)
	{
		ResourceLayoutElementDescription[] elements = new ResourceLayoutElementDescription[Resources.Length];
		for (int i = 0; i < Resources.Length; ++i)
		{
			elements[i] = Resources[i].CreateResourceLayoutElementDescription(stages);
		}

		return new ResourceLayoutDescription(elements);
	}

	private ResourceSetDescription GetResourceSetDescription(ShaderStages stages)
	{
		BindableResource[] bindableResources = new BindableResource[Resources.Length];
		for (int i = 0; i < Resources.Length; ++i)
		{
			bindableResources[i] = Resources[i].GetOrCreateBindableResource();
		}

		return new ResourceSetDescription(GetResourceLayout(stages), bindableResources);
	}

	public void Dispose()
	{
		_resourceSet.Dispose();
		_resourceLayout.Dispose();

		for (int i = 0; i < Resources.Length; ++i)
		{
			Resources[i].Dispose();
		}
	}
}