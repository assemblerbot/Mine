namespace Mine.Framework;

public sealed class ManagedList<T>
{
	private readonly List<T> _items = new();
	private readonly List<T> _toRemove = new();
	
	public void ForEach(Action<T> action)
	{
		for(int i=0;i<_items.Count;++i)
		{
			action(_items[i]);
		}

		Cleanup();
	}

	public void Add(T item)
	{
		_items.Add(item);
	}

	public void Insert(T item, Func<T,int> orderFromItem)
	{
		int order = orderFromItem(item);
		int index = _items.FindInsertionIndexBinary(x => orderFromItem(x).CompareTo(order));
		_items.Insert(index, item);
	}
	
	public void Remove(T item)
	{
		_toRemove.Add(item);
	}

	private void Cleanup()
	{
		if (_toRemove.Count == 0)
		{
			return;
		}

		for(int i=0;i <_toRemove.Count; ++i)
		{
			int index = _items.IndexOf(_toRemove[i]);
			if (index == -1)
			{
				continue;
			}

			_items.RemoveAt(index);
		}

		_toRemove.Clear();
	}
}