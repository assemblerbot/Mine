namespace Mine.Studio;

public sealed class SelectionViewModel
{
	#region Events
	public struct SelectionChanged : IStudioModelEvent { }
	#endregion
	
	private readonly StudioModelEventAggregator                     _eventAggregator;
	private readonly Dictionary<string, WeakReference<ISelectable>> _selection = new();
	
	public SelectionViewModel(StudioModelEventAggregator eventAggregator)
	{
		_eventAggregator = eventAggregator;
	}

	public void Select(string id, ISelectable target)
	{
		_selection[id] = new WeakReference<ISelectable>(target);
		_eventAggregator.Trigger(new SelectionChanged());
	}
	
	public void Deselect(string id)
	{
		_selection.Remove(id);
		_eventAggregator.Trigger(new SelectionChanged());
	}

	public void Flip(string id, ISelectable target)
	{
		if (_selection.ContainsKey(id))
		{
			Deselect(id);
		}
		else
		{
			Select(id, target);
		}
	}

	public void DeselectAll()
	{
		_selection.Clear();
		_eventAggregator.Trigger(new SelectionChanged());
	}

	public bool IsSelected(string id)
	{
		return _selection.TryGetValue(id, out WeakReference<ISelectable>? targetRef) && targetRef.TryGetTarget(out _);
	}

	public object? SelectedTarget(string id)
	{
		return _selection.TryGetValue(id, out WeakReference<ISelectable>? targetRef) ? (targetRef.TryGetTarget(out ISelectable? target) ? target : null) : null;
	}
	
	public IReadOnlyList<ISelectable> GetAllSelectedTargets()
	{
		return _selection.Values.Select(reference => reference.TryGetTarget(out ISelectable? target) ? target : null).Where(target => target != null).ToList()!;
	}

	public object? GetFirstSelectedTarget()
	{
		foreach (WeakReference<ISelectable> reference in _selection.Values)
		{
			if (reference.TryGetTarget(out ISelectable? target))
			{
				return target;
			}
		}

		return null;
	}

	public int GetSelectedCount()
	{
		return GetAllSelectedTargets().Count;
	}

	public void SaveAllSelectedTargets()
	{
		
	}
}