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


	public void Select(string path, object target)
	{
		_selection[path] = new WeakReference<object>(target);
		_eventAggregator.Trigger(new SelectionChanged());
	}
	
	public void Deselect(string path)
	{
		_selection.Remove(path);
		_eventAggregator.Trigger(new SelectionChanged());
	}

	public void Flip(string path, object target)
	{
		if (_selection.ContainsKey(path))
		{
			Deselect(path);
		}
		else
		{
			Select(path, target);
		}
	}

	public void DeselectAll()
	{
		_selection.Clear();
		_eventAggregator.Trigger(new SelectionChanged());
	}

	public bool IsSelected(string item)
	{
		return _selection.ContainsKey(item);
	}

	public object? SelectedTarget(string item)
	{
		return _selection.TryGetValue(item, out WeakReference<object>? targetRef) ? (targetRef.TryGetTarget(out object? target) ? target : null) : null;
	}
	
	public IReadOnlyList<object> GetAllSelectedTargets()
	{
		return _selection.Values.Select(reference => reference.TryGetTarget(out object? target) ? target : null).Where(target => target != null).ToList()!;
	}
}