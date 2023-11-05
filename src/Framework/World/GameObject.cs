using System.Numerics;

namespace GameToolkit.Framework;

public sealed class GameObject
{
	private string _name;
	public  string Name => _name;
	
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

	#region Component management
	public GameObject AddComponent(Component component)
	{
		component.SetGameObject(this);
		_components.Add(component);
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
}