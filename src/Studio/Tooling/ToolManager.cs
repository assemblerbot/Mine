using GameToolkit.Framework;
using RedHerring.Studio.Models;

namespace RedHerring.Studio.Tools;

public sealed class ToolManager
{
	private readonly Dictionary<string, Type> _toolsByName = new();
	private readonly List<ATool>              _activeTools = new();

	private StudioModel _studioModel;
	
	public void Init(StudioModel studioModel)
	{
		_studioModel    = studioModel;
		ScanTools();
	}

	public ATool? Activate(string toolName, int uniqueId = -1)
	{
		ATool? tool = uniqueId == -1
				? (ATool?) Activator.CreateInstance(_toolsByName[toolName], _studioModel)
				: (ATool?) Activator.CreateInstance(_toolsByName[toolName], _studioModel, uniqueId);
		if (tool == null)
		{
			return null;
		}
		
		_activeTools.Add(tool);
		return tool;
	}

	public void Update()
	{
		for(int i=0;i <_activeTools.Count;++i)
		{
			_activeTools[i].Update(out bool finished);
			if (finished)
			{
				_activeTools.RemoveAt(i);
				--i;
			}
		}
	}

	#region Import/Export
	public List<ToolId> ExportActiveTools()
	{
		return _activeTools.Select(tool => tool.Id).ToList();
	}

	public void ImportActiveTools(List<ToolId>? toolIds)
	{
		_activeTools.Clear();

		if (toolIds == null)
		{
			return;
		}

		foreach (ToolId id in toolIds)
		{
			Activate(id.Name, id.UniqueId);
		}
	}
	#endregion

	private void ScanTools()
	{
		Engine.Types.ForEachAttribute<ToolAttribute>((attribute, type) => _toolsByName.Add(attribute.Name, type));
	}
}