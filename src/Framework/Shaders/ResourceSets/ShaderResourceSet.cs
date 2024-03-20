using Veldrid;

namespace Mine.Framework;

// TODO - change to abstract
//  this class will be custom resource set used in material, it can and should manage its own layout
//  built-in resource sets will have fixed structure and their layouts will be stored in Shared
public class ShaderResourceSet : IDisposable
{
	public readonly ShaderResource[] Resources;

	private ResourceLayout? _resourceLayout;
	private ResourceSet?    _resourceSet;

	public ResourceLayout ResourceLayout => GetOrCreateResourceLayout();
	public ResourceSet    ResourceSet    => GetOrCreateResourceSet();

	public ShaderResourceSet(params ShaderResource[] resources)
	{
		Resources = resources;
	}

	private ResourceLayout GetOrCreateResourceLayout()
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

	private ResourceSet GetOrCreateResourceSet()
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

		ResourceSetDescription description = new(GetOrCreateResourceLayout(), bindableResources);
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

	public virtual void Update()
	{
	}
}