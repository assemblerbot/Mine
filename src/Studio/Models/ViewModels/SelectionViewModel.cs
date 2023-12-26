namespace RedHerring.Studio.Models.ViewModels;

public sealed class SelectionViewModel
{
	#region Events
	public struct SelectionChanged : IStudioModelEvent { }
	#endregion
	
	private readonly StudioModelEventAggregator                _eventAggregator;
	private          Dictionary<string, WeakReference<object>> _selection = new();
	
	public SelectionViewModel(StudioModelEventAggregator eventAggregator)
	{
		_eventAggregator = eventAggregator;
	}

	public void Select(string id, object target)
	{
		_selection[id] = new WeakReference<object>(target);
		_eventAggregator.Trigger(new SelectionChanged());
	}
	
	public void Deselect(string id)
	{
		_selection.Remove(id);
		_eventAggregator.Trigger(new SelectionChanged());
	}

	public void Flip(string id, object target)
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
		return _selection.TryGetValue(id, out WeakReference<object>? targetRef) && targetRef.TryGetTarget(out _);
	}

	public object? SelectedTarget(string id)
	{
		return _selection.TryGetValue(id, out WeakReference<object>? targetRef) ? (targetRef.TryGetTarget(out object? target) ? target : null) : null;
	}
	
	public IReadOnlyList<object> GetAllSelectedTargets()
	{
		return _selection.Values.Select(reference => reference.TryGetTarget(out object? target) ? target : null).Where(target => target != null).ToList()!;
	}

	public object? GetFirstSelectedTarget()
	{
		foreach (WeakReference<object> reference in _selection.Values)
		{
			if (reference.TryGetTarget(out object? target))
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
}