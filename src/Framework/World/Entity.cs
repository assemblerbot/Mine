namespace Mine.Framework;

public sealed partial class Entity : IDisposable
{
	private string _name;
	public  string Name => _name;

	private EntityFlags _flags = EntityFlags.Default;
	public  EntityFlags Flags => _flags;

	public bool ActiveSelf        => IsFlags(EntityFlags.ActiveSelf);
	public bool ActiveInHierarchy => ActiveSelf && (_parent is null || _parent.ActiveInHierarchy);

	public bool IsInWorld => IsFlags(EntityFlags.IsInWorld);
	public bool IsDisposed => IsFlags(EntityFlags.IsDisposed);
	
	// hierarchy
	private Entity? _parent = null;
	public  Entity? Parent => _parent;
	
	private List<Entity>          _children = new();
	public  IReadOnlyList<Entity> Children => _children;
	
	// components
	private List<Component> _components = new();
	
	public Entity(string name = "(Entity)")
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
		SetFlags(EntityFlags.IsDisposed);

		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].Dispose();
		}
		_children.Clear();
		
		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].Dispose();
		}
		_components.Clear();

		_shaderResourceWorldMatrix?.Dispose();
	}

	#region Activity
	public void SetActive(bool active)
	{
		if (active)
		{
			SetFlags(EntityFlags.ActiveSelf);
		}
		else
		{
			ClearFlags(EntityFlags.ActiveSelf);
		}
	}
	#endregion
	
	#region Hierarchy
	public Entity? GetChild(string path)
	{
		string[] pathChunks = path.Split('/');

		Entity? current = this;
		foreach (string chunk in pathChunks)
		{
			if (string.IsNullOrEmpty(chunk))
			{
				continue;
			}

			current = current._children.FirstOrDefault(child => child.Name == chunk);
			if (current is null)
			{
				return null;
			}
		}

		return current;
	}

	public void AddChild(Entity entity)
	{
		bool wasInWorld = entity.IsInWorld;
		if (wasInWorld && !IsInWorld)
		{
			entity.BeforeRemovedFromWorld();
		}

		entity.SetParent(this);

		if (!wasInWorld && IsInWorld)
		{
			entity.AfterAddedToWorld();
		}
	}

	public void RemoveChild(Entity entity)
	{
		if (entity.IsInWorld)
		{
			entity.BeforeRemovedFromWorld();
		}

		entity.SetParent(null);
	}

	public void Destroy()
	{
		if (_parent is not null)
		{
			_parent.DestroyChild(this);
		}
		else
		{
			Dispose();
		}
	}
	
	public void DestroyChild(Entity entity)
	{
		RemoveChild(entity);
		entity.Dispose();
	}

	public void DestroyChildren()
	{
		while (Children.Count > 0)
		{
			DestroyChild(Children[^1]);
		}
	}

	private void SetParent(Entity? parent)
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

		_components.RemoveAt(index);
	}
	
	#endregion

	#region Internal
	private void AfterAddedToWorld()
	{
		SetFlags(EntityFlags.IsInWorld);
		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].AfterAddedToWorld();
		}
		
		for (int i = 0; i < _children.Count; ++i)
		{
			if (!_children[i].IsInWorld)
			{
				_children[i].AfterAddedToWorld();
			}
		}
	}

	private void BeforeRemovedFromWorld()
	{
		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].BeforeRemovedFromWorld();
		}

		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].BeforeRemovedFromWorld();
		}
		ClearFlags(EntityFlags.IsInWorld);
	}

	internal void SetFlags(EntityFlags flags)
	{
		EntityFlags oldFlags = _flags;
		_flags |= flags;
		if (_flags != oldFlags)
		{
			NotifyComponentsAboutFlagsChanged(oldFlags);
		}
	}

	internal void ClearFlags(EntityFlags flags)
	{
		EntityFlags oldFlags = _flags;
		_flags &= ~flags;
		if (_flags != oldFlags)
		{
			NotifyComponentsAboutFlagsChanged(oldFlags);
		}
	}

	internal bool IsFlags(EntityFlags flags)
	{
		return (_flags & flags) == flags;
	}

	private void PropagateFlagsToChildren(EntityFlags flags)
	{
		SetFlags(flags);
		for (int i = 0; i < Children.Count; ++i)
		{
			Children[i].PropagateFlagsToChildren(flags);
		}
	}

	private void NotifyComponentsAboutFlagsChanged(EntityFlags oldFlags)
	{
		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].AfterEntityFlagsChanged(oldFlags, _flags);
		}
	}
	#endregion
}