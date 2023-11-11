using System.Numerics;

namespace GameToolkit.Framework;

public sealed class GameObject
{
	private string _name;
	public  string Name => _name;

	private bool _activeSelf        = true;
	private bool _activeInHierarchy = true;
	public  bool ActiveSelf        => _activeSelf;
	public  bool ActiveInHierarchy => _activeInHierarchy;
	
	private Vector3    _position;
	private Quaternion _rotation;
	
	private GameObject?      _parent   = null;
	private List<GameObject> _children = new();
	
	private List<Component> _components = new();

	public GameObject()
	{
		_name = "(GameObject)";
	}

	public GameObject(string name)
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
		if (_activeSelf == active)
		{
			return;
		}

		_activeSelf = active;

		if (!_activeInHierarchy)
		{
			return;
		}
		
		UpdateActiveInHierarchyRecursive(true);
	}

	private void UpdateActiveInHierarchyRecursive(bool parentIsActive)
	{
		_activeInHierarchy = parentIsActive && _activeSelf;
		
		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].UpdateActiveInHierarchyRecursive(_activeInHierarchy);
		}
	}
	#endregion

	#region Component management
	public GameObject AddComponent<T>() where T : Component, new()
	{
		Component component = new T();
		
		component.SetGameObject(this);
		_components.Add(component);

		component.OnInstantiate();
		return this;
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
	protected internal void AfterAddedToScene()
	{
		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].AfterAddedToScene();
		}
		
		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].AfterAddedToScene();
		}
	}

	protected internal void BeforeRemovedFromScene()
	{
		for (int i = 0; i < _children.Count; ++i)
		{
			_children[i].BeforeRemovedFromScene();
		}

		for (int i = 0; i < _components.Count; ++i)
		{
			_components[i].BeforeRemovedFromScene();
		}
	}

	#endregion
}