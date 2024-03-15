using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSet : IDisposable
{
	public readonly ShaderResource[] Resources;

	private ResourceLayout? _resourceLayout;
	private ResourceSet?    _resourceSet;

	public ShaderResourceSet(params ShaderResource[] resources)
	{
		Resources = resources;
	}

	public ResourceLayout GetOrCreateResourceLayout()
	{
		if (_resourceLayout is not null)
		{
			return _resourceLayout;
		}

		ResourceLayoutElementDescription[] elements = new ResourceLayoutElementDescription[Resources.Length];
		for (int i = 0; i < Resources.Length; ++i)
		{
			elements[i] = Resources[i].CreateResourceLayoutElementDescription();
		}

		ResourceLayoutDescription description = new(elements);
		_resourceLayout = Engine.Graphics.Factory.CreateResourceLayout(description);
		return _resourceLayout;
	}

	public ResourceSet GetOrCreateResourceSet()
	{
		if (_resourceSet is not null)
		{
			return _resourceSet;
		}

		BindableResource[] bindableResources = new BindableResource[Resources.Length];
		for (int i = 0; i < Resources.Length; ++i)
		{
			bindableResources[i] = Resources[i].GetOrCreateBindableResource();
		}

		ResourceSetDescription description = new (GetOrCreateResourceLayout(), bindableResources);
		_resourceSet = Engine.Graphics.Factory.CreateResourceSet(description);
		return _resourceSet;
	}

	public void Dispose()
	{
		_resourceSet?.Dispose();
		_resourceSet = null;

		_resourceLayout?.Dispose();
		_resourceLayout = null;

		for (int i = 0; i < Resources.Length; ++i)
		{
			Resources[i].Dispose();
		}
	}
}