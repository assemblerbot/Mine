namespace Mine.Framework;

public sealed partial class Entity
{
	private string _name;
	public  string Name => _name;

	private bool _activeSelf        = true;
	public  bool ActiveSelf        => _activeSelf;
	public  bool ActiveInHierarchy => _activeSelf && (_parent is null || _parent.ActiveInHierarchy);

	// hierarchy
	private Entity?      _parent = null;
	public  Entity?      Parent => _parent;
	
	private List<Entity>          _children = new();
	public  IReadOnlyList<Entity> Children => _children;
	
	// components
	private List<Component> _components = new();
	
	// layers
	public ulong RenderingLayers = 0;

	public Entity()
	{
		_name = "(Entity)";
	}

	public Entity(string name)
	{
		_name = name;
	}
	
	public void CallOnComponents<TComponent>(Action<TComponent> func) where TComponent : Component
	{
		foreach (Component component in _components)
		{
			if (component is TComponent castedComponent)
			{
				func(castedComponent);
			}
		}
	}

	public void CallOnComponentsInHierarchy<TComponent>(Action<TComponent> func) where TComponent : Component
	{
		CallOnComponents(func);

		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].CallOnComponentsInHierarchy(func);
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].Dispose();
		}
		_components.Clear();

		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].Dispose();
		}
		_children.Clear();
	}

	#region Activity
	public void SetActive(bool active)
	{
		_activeSelf = active;
	}
	#endregion
	
	#region Hierarchy
	public void SetParent(Entity? parent)
	{
		if (_parent is not null)
		{
			_parent._children.Remove(this);
			_parent = null;
		}

		_parent = parent;
		if (_parent == null)
		{
			return;
		}

		_parent._children.Add(this);
	}
	#endregion

	#region Component management
	public T AddComponent<T>() where T : Component, new()
	{
		return AddComponent(new T());
	}

	public T AddComponent<T>(T component) where T : Component
	{
		component.SetEntity(this);
		_components.Add(component);

		component.OnInstantiate();
		return component;
	}

	public void RemoveComponent(Component component)
	{
		int index = _components.IndexOf(component);
		if (index == -1)
		{
			// TODO - log error
			return;
		}

		_components.RemoveAt(index); // TODO - not sure if this should be called here or posponed until end of Update
	}
	#endregion

	#region Scene Add/Remove
	internal void AfterAddedToWorld()
	{
		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].AfterAddedToWorld();
		}
		
		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].AfterAddedToWorld();
		}
	}

	internal void BeforeRemovedFromWorld()
	{
		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].BeforeRemovedFromWorld();
		}

		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].BeforeRemovedFromWorld();
		}
	}

	#endregion
}