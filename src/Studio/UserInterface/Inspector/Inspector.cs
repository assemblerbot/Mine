using CommandProcessorPlugin;

namespace Mine.Studio;

public class Inspector : IInspector
{
	private InspectorControlMap _controlMap = new ();
	public virtual InspectorControlMap ControlMap => _controlMap;
	
	private readonly List<object>           _sources = new();
	private          InspectorClassControl? _contentControl;

	private static int _uniqueIdGenerator = 0;
	protected        int _uniqueId          = _uniqueIdGenerator++;

	private ICommandHistory _commandHistory;

	public Inspector(ICommandHistory commandHistory)
	{
		_commandHistory = commandHistory;
	}

	public void Init(object? source)
	{
		_sources.Clear();
		
		if (source != null)
		{
			_sources.Add(source);
		}

		Rebuild();
	}

	public void Init(IReadOnlyCollection<object> sources)
	{
		_sources.Clear();
		_sources.AddRange(sources);
		Rebuild();
	}

	public virtual void Update()
	{
		_contentControl?.Update();
	}

	public void Commit(Command command)
	{
		_commandHistory.Commit(command);
	}

	#region Private
	private void Rebuild()
	{
		_contentControl = null;
		if (_sources.Count == 0)
		{
			return;
		}

		_contentControl = new InspectorClassControl(this, _uniqueId.ToString());
		_contentControl.InitFromSource(null, _sources[0]);
		_contentControl.SetCustomLabel(null);
		for(int i=1;i <_sources.Count;i++)
		{
			_contentControl.AdaptToSource(null, _sources[i]);
		}
	}
	#endregion
}
