using System.Numerics;
using ImGuiNET;
using Mine.Framework;
using RedHerring.Studio.Models;
using RedHerring.Studio.Tools;

namespace Mine.Studio;

[Tool(ToolName)]
public class ToolPlugins : Tool
{
	public const       string ToolName = "Plugins";
	protected override string Name => ToolName;

	private readonly StudioModel _studioModel;
	
	private readonly PluginManagerSettings                    _settings;
	private readonly PluginManagerCollections                 _pluginCollections;
	private readonly Dictionary<string, PluginManagerFoldout> _pluginFoldouts = new();

	private bool _refreshRequested = false;

	public ToolPlugins(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
		_studioModel       = studioModel;
		_settings          = new PluginManagerSettings();
		_pluginCollections = new PluginManagerCollections();
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
			_pluginFoldouts.Add(pluginsPair.Id, new PluginManagerFoldout(_settings, _pluginCollections, pluginsPair, RequestRefresh));
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
			RequestRefresh();
		}

		return !isOpen;
	}

	private void SettingsUI()
	{
		ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
		
		string settingsPath = PluginManagerSettings.StudioPluginsSettingsPath;
		ImGui.InputText("Settings file", ref settingsPath, (uint)settingsPath.Length);

		string? repositoryPath = _settings.RepositoryPath;
		if (repositoryPath != null)
		{
			ImGui.InputText("Repository path", ref repositoryPath, (uint) repositoryPath.Length);
		}

		string? projectPath = _settings.ProjectPath;
		if (projectPath != null)
		{
			ImGui.InputText("Project path", ref projectPath, (uint) projectPath.Length);
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
		if (ImGui.Button(FontAwesome6.ArrowsRotate))
		{
			RequestRefresh();
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
}