using System.Numerics;
using ImGuiNET;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Tools;

namespace Mine.Studio;

[Tool(ToolName)]
public class ToolPlugins : Tool
{
	public const       string ToolName = FontAwesome6.Plug + " Plugins";
	protected override string Name => ToolName;

	private readonly StudioModel _studioModel;
	
	private readonly PluginManagerSettings                    _settings;
	private readonly PluginManagerCollections                 _pluginCollections;
	private readonly Dictionary<string, PluginManagerFoldout> _pluginFoldouts = new();

	private bool _eventsRegistered = false;
	private bool _refreshRequested = false;

	private readonly string _refreshButtonTitleId;

	public ToolPlugins(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
		_studioModel          = studioModel;
		_settings             = new PluginManagerSettings();
		_pluginCollections    = new PluginManagerCollections();
		_refreshButtonTitleId = $"{FontAwesome6.ArrowsRotate}##{uniqueId}.Refresh";
		
		Refresh();
	}
	
	public override void Update(out bool finished)
	{
		finished = UpdateUI();
	}

	private void Refresh()
	{
		_pluginFoldouts.Clear();

		_settings.Refresh(_studioModel.Project);
		if (_settings.IsError)
		{
			return;
		}

		_pluginCollections.Refresh(_settings);

		if (_pluginCollections.IsError)
		{
			return;
		}

		List<PluginManagerCollections.CPluginsPair> listOfPlugins = _pluginCollections.BuildListOfPlugins();
		foreach (PluginManagerCollections.CPluginsPair pluginsPair in listOfPlugins)
		{
			_pluginFoldouts.Add(pluginsPair.Id, new PluginManagerFoldout(_studioModel, _settings, _pluginCollections, pluginsPair, RequestRefresh));
		}
	}

	private void RequestRefresh()
	{
		_refreshRequested = true;
	}

	#region UI
	private bool UpdateUI()
	{
		bool isOpen = true;
		if (ImGui.Begin(NameId, ref isOpen))
		{
			if (!_eventsRegistered)
			{
				RegisterEvents();
			}

			if (_refreshRequested)
			{
				_refreshRequested = false;
				Refresh();
			}

			SettingsUI();
			RefreshButtonUI();
			PluginsUI();
			ImGui.End();
		}
		else
		{
			if (_eventsRegistered)
			{
				UnregisterEvents();
			}

			RequestRefresh();
		}

		return !isOpen;
	}

	private void SettingsUI()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().DisabledAlpha);
		
		string settingsPath = PluginManagerSettings.StudioPluginsSettingsPath;
		ImGui.InputText("Settings file", ref settingsPath, (uint)settingsPath.Length);

		string? repositoryPath = _settings.RepositoryPath;
		if (repositoryPath != null)
		{
			ImGui.InputText("Repository path", ref repositoryPath, (uint) repositoryPath.Length);
		}

		string? projectScriptsPath = _settings.ProjectScriptsPath;
		if (projectScriptsPath != null)
		{
			ImGui.InputText("Project scripts path", ref projectScriptsPath, (uint) projectScriptsPath.Length);
		}

		string? projectAssetsPath = _settings.ProjectAssetsPath;
		if (projectAssetsPath != null)
		{
			ImGui.InputText("Project assets path", ref projectAssetsPath, (uint) projectAssetsPath.Length);
		}
		
		ImGui.PopStyleVar();

		string? errorMessage = _settings.ErrorMessage;
		if (errorMessage != null)
		{
			ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 0, 0, 1));
			ImGui.Text(errorMessage);
			ImGui.PopStyleColor();
		}
	}

	private void RefreshButtonUI()
	{
		if (ImGui.Button(_refreshButtonTitleId))
		{
			RequestRefresh();
			ImGui.OpenPopup("asdf");
		}
	}
	
	private void PluginsUI()
	{
		if (_pluginCollections.IsError)
		{
			ImGui.Separator();
			ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 0, 0, 1));
			ImGui.Text(_pluginCollections.ErrorMessage);
			ImGui.PopStyleColor();
			return;
		}
		
		if (ImGui.BeginTable("PluginsTable", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
		{
			ImGui.TableSetupColumn("",               ImGuiTableColumnFlags.WidthFixed);
			ImGui.TableSetupColumn("Name & version", ImGuiTableColumnFlags.WidthStretch);
			ImGui.TableSetupColumn("Installation",   ImGuiTableColumnFlags.WidthFixed);
			ImGui.TableSetupColumn("Uninstallation", ImGuiTableColumnFlags.WidthFixed);
			ImGui.TableHeadersRow();
			
			foreach (PluginManagerFoldout foldout in _pluginFoldouts.Values)
			{
				foldout.UpdateUI();
			}

			ImGui.EndTable();
		}
	}
	#endregion
	
	#region Event handlers
	private void RegisterEvents()
	{
		_studioModel.EventAggregator.Register<ProjectModel.OpenedEvent>(OnProjectOpened);
		_studioModel.EventAggregator.Register<ProjectModel.ClosedEvent>(OnProjectClosed);
		_eventsRegistered = true;
	}
	
	private void UnregisterEvents()
	{
		_studioModel.EventAggregator.Unregister<ProjectModel.OpenedEvent>(OnProjectOpened);
		_studioModel.EventAggregator.Unregister<ProjectModel.ClosedEvent>(OnProjectClosed);
		_eventsRegistered = false;
	}

	private void OnProjectOpened(ProjectModel.OpenedEvent e)
	{
		RequestRefresh();
	}
	
	private void OnProjectClosed(ProjectModel.ClosedEvent e)
	{
		RequestRefresh();
	}
	#endregion
}