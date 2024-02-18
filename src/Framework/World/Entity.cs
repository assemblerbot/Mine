namespace Mine.Framework;

public sealed class Entity
{
	private string _name;
	public  string Name => _name;

	private bool _activeSelf        = true;
	public  bool ActiveSelf        => _activeSelf;
	public  bool ActiveInHierarchy => _activeSelf && _parent is not null && _parent.ActiveInHierarchy;

	private EntityFlags     _flags;

	// hierarchy
	private Entity?      _parent   = null;
	private List<Entity> _children = new();
	
	// components
	private List<Component> _components = new();

	// transform
	public  Point3Float     LocalPosition      { get; set; }         = Point3Float.Zero;
	public  QuaternionFloat LocalRotation      { get; set; }         = QuaternionFloat.Identity;
	public  Vector3Float    LocalScale         { get; set; }         = Vector3Float.One;

	// public Point3Float     WorldPosition => LocalToWorldMatrix.Transform(LocalPosition);
	// public QuaternionFloat WorldRotation => LocalToWorldMatrix.TransForm(LocalRotation);
	
	public  Matrix4x4Float  LocalToWorldMatrix { get; private set; } = Matrix4x4Float.Identity;
	public  Matrix4x4Float  WorldToLocalMatrix { get; private set; } = Matrix4x4Float.Identity;
	private bool            _matricesDirty = true;
	
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

	#region Component management
	public T AddComponent<T>() where T : Component, new()
	{
		Component component = new T();
		
		component.SetEntity(this);
		_components.Add(component);

		component.OnInstantiate();
		return (component as T)!;
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