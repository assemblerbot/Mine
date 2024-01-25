using System.Numerics;
using ImGuiNET;
using Mine.Framework;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Models;

namespace Mine.Studio;

public class PluginManagerFoldout
{
	private readonly StudioModel _studioModel;
	
	private readonly PluginManagerSettings    _settings;
	private readonly PluginManagerCollections _collections;
	private          Action                   _onChange;

	private readonly PluginManagerPlugin? _repositoryPlugin;
	private readonly PluginManagerPlugin? _projectPlugin;
	private readonly PluginManagerPlugin  _plugin;
	
	private readonly string                _repositoryVersion;
	private readonly string                _projectVersion;
	private readonly string                _titleId;

	private readonly List<PluginManagerPlugin> _pluginsDependentOnThis;

	private readonly bool _isDependencyError;
	private readonly bool _isVersionError;
	private          bool IsError => _isDependencyError || _isVersionError;

	private readonly string? _buttonNameIdInstall;
	private readonly string? _buttonNameIdUninstall;
	private readonly string? _buttonNameIdUpgrade;
	private readonly string? _buttonNameIdDowngrade;
	private readonly string? _buttonNameIdCopyToRepository;
	private readonly string? _buttonNameIdCreateInRepository;
	
	private readonly string? _textRequiredByOtherPlugins;
	private readonly string? _textUpToDate;
	
	public PluginManagerFoldout(
		StudioModel studioModel,
		PluginManagerSettings                 settings,
		PluginManagerCollections              collections,
		PluginManagerCollections.CPluginsPair pluginsPair,
		Action                                onChange
	)
	{
		_studioModel = studioModel;
		_settings    = settings;
		_collections = collections;
		_onChange    = onChange;

		// gather resources
		_repositoryPlugin = pluginsPair.RepositoryPlugin;
		_projectPlugin    = pluginsPair.ProjectPlugin;
		_plugin           = _projectPlugin ?? _repositoryPlugin!;

		_repositoryVersion = _repositoryPlugin?.Version ?? "not available";
		_projectVersion    = _projectPlugin?.Version    ?? "not installed";
		_titleId           = $"{(_plugin.IsAssetPlugin ? FontAwesome6.Cubes : FontAwesome6.Code)} {_plugin.Name} ({_projectVersion})##{_plugin.Id}.title";

		_pluginsDependentOnThis = collections.GetAllProjectPluginsDependentOn(_plugin.Id);
		_isDependencyError      = collections.IsDependencyError(_plugin, _projectPlugin != null);
		_isVersionError         = (_repositoryPlugin?.ParseVersion.IsError ?? false) || (_projectPlugin?.ParseVersion.IsError ?? false);

		// status
		if (IsError)
		{
			_buttonNameIdUninstall = $"Uninstall##{_plugin.Id}.uninstall";
			return;
		}

		if (_repositoryPlugin != null)
		{
			if (_projectPlugin == null)
			{
				_buttonNameIdInstall = $"Install##{_plugin.Id}.install";
			}
			else
			{
				PluginManagerVersion repositoryVersionNumber = _repositoryPlugin.ParseVersion;
				PluginManagerVersion projectVersionNumber    = _projectPlugin.ParseVersion;
				string                     apiBreak = PluginManagerVersion.IsApiBreak(repositoryVersionNumber, projectVersionNumber) ? " INCOMPATIBLE" : "";

				if (projectVersionNumber < repositoryVersionNumber || projectVersionNumber == repositoryVersionNumber)
				{
					if (projectVersionNumber < repositoryVersionNumber)
					{
						_buttonNameIdUpgrade = $"Upgrade to {_repositoryPlugin.Version}{apiBreak}##{_plugin.Id}.upgrade";
					}
					else
					{
						_textUpToDate = "Up to date.";
					}

					if (_pluginsDependentOnThis.Count > 0)
					{
						_textRequiredByOtherPlugins = "Required by other plugin(s).";
					}
					else
					{
						_buttonNameIdUninstall = $"Uninstall##{_plugin.Id}.uninstall";
					}
				}
				else
				{
					_buttonNameIdDowngrade = $"Downgrade to {_repositoryPlugin.Version}{apiBreak}##{_plugin.Id}.downgrade";
					_buttonNameIdCopyToRepository = $"Copy to repository##{_plugin.Id}.copy";
				}
			}
		}
		else if (_projectPlugin != null)
		{
			_buttonNameIdCreateInRepository = $"Create in repository##{_plugin.Id}.create";
		}
	}

	#region UI
	public void UpdateUI()
	{
		ImGui.TableNextRow();
		
		ImGui.TableSetColumnIndex(0);
		StatusIconUI();

		ImGui.TableSetColumnIndex(1);
		DescriptionUI();

		ImGui.TableSetColumnIndex(2);
		InstallationUI();

		ImGui.TableSetColumnIndex(3);
		UninstallationUI();
	}

	private void StatusIconUI()
	{
		PluginManagerStatusIcons.GetPluginStatusIcon(_repositoryPlugin, _projectPlugin, out string icon, out Vector4 color);
		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.Text(icon);
		ImGui.PopStyleColor();
	}

	private void DescriptionUI()
	{
		if (IsError)
		{
			ImGui.SetNextItemOpen(true);
		}

		if (ImGui.CollapsingHeader(_titleId))
		{
			ImGui.Indent();
			ImGui.Text(_plugin.Description);

			ThisDependsOnUI();
			OtherDependentOnThisUI();
			ImGui.Spacing();
			
			ImGui.Text("Id:");
			ImGui.SameLine();
			ImGui.Text(_plugin.Id);

			ImGui.Text("Repository:");
			ImGui.SameLine();
			ImGui.Text(_repositoryVersion);

			// errors
			if (_repositoryPlugin != null && !string.IsNullOrEmpty(_repositoryPlugin.Error))
			{
				ErrorText(_repositoryPlugin.Error);
			}
			
			if (_projectPlugin != null && !string.IsNullOrEmpty(_projectPlugin.Error))
			{
				ErrorText(_projectPlugin.Error);
			}
			
			if (_isVersionError)
			{
				ErrorText("Plugin has invalid version format!");
			}
			
			if (_isDependencyError)
			{
				ErrorText("Plugin is dependent on non existing plugin!");
			}

			ImGui.Unindent();
		}
	}

	private void InstallationUI()
	{
		if (_buttonNameIdInstall != null)
		{
			if (ImGui.Button(_buttonNameIdInstall))
			{
				Install();
			}
			return;
		}

		if (_buttonNameIdUpgrade != null)
		{
			if (ImGui.Button(_buttonNameIdUpgrade))
			{
				Upgrade();
			}
			return;
		}

		if (_buttonNameIdCopyToRepository != null)
		{
			if (ImGui.Button(_buttonNameIdCopyToRepository))
			{
				CopyToRepository();
			}
			return;
		}

		if (_buttonNameIdCreateInRepository != null)
		{
			if(ImGui.Button(_buttonNameIdCreateInRepository))
			{
				CreateInRepository();
			}
			return;
		}

		if (_textUpToDate != null)
		{
			ImGui.Text(_textUpToDate);
		}
	}

	private void UninstallationUI()
	{
		if (_buttonNameIdUninstall != null)
		{
			if (ImGui.Button(_buttonNameIdUninstall))
			{
				Uninstall();
			}
			return;
		}

		if (_buttonNameIdDowngrade != null)
		{
			if (ImGui.Button(_buttonNameIdDowngrade))
			{
				Downgrade();
			}
			return;
		}

		if (_textRequiredByOtherPlugins != null)
		{
			ImGui.Text(_textRequiredByOtherPlugins);
		}
	}

	private void ThisDependsOnUI()
	{
		if (_plugin.Dependencies != null && _plugin.Dependencies.Count > 0)
		{
			ImGui.Text("Required plugins:");
			ImGui.Indent();
			foreach (string dependency in _plugin.Dependencies)
			{
				CreatePluginDescriptionRow(dependency);
			}
			ImGui.Unindent();
		}
	}

	private void OtherDependentOnThisUI()
	{
		if (_pluginsDependentOnThis.Count == 0)
		{
			return;
		}

		ImGui.Text("Required by plugins:");
		ImGui.Indent();
		foreach (PluginManagerPlugin plugin in _pluginsDependentOnThis)
		{
			CreatePluginDescriptionRow(plugin.Id);
		}
		ImGui.Unindent();
	}

	private void CreatePluginDescriptionRow(string pluginId)
	{
		PluginManagerStatusIcons.GetDependencyStatusIcon(
			_collections.IsInProject(pluginId),
			_collections.IsInRepository(pluginId),
			out string icon,
			out Vector4 color
		);

		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.Text(icon);
		ImGui.PopStyleColor();

		ImGui.SameLine();
		ImGui.Text(_collections.GetPluginName(pluginId));

		ImGui.SameLine();
		ImGui.Text(_collections.GetPluginVersion(pluginId));
	}

	private void ErrorText(string text)
	{
		ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 0, 0, 1));
		ImGui.Text(text);
		ImGui.PopStyleColor();
	}
	#endregion

	#region Manipulation
	private void Install()
	{
		_studioModel.Project.PauseWatchers();
		_collections.CopyDependenciesFromRepositoryToProject(_repositoryPlugin!, _settings);
		_repositoryPlugin!.CopyFromRepositoryToProject(_settings);
		_studioModel.Project.ResumeWatchers();
		_onChange();
	}

	private void Upgrade()
	{
		_studioModel.Project.PauseWatchers();
		_collections.CopyDependenciesFromRepositoryToProject(_repositoryPlugin!, _settings);
		_repositoryPlugin!.CopyFromRepositoryToProject(_settings);
		_studioModel.Project.ResumeWatchers();
		_onChange();
	}

	private void CopyToRepository()
	{
		_studioModel.Project.PauseWatchers();
		_projectPlugin!.CopyFromProjectToRepository(_settings);
		_studioModel.Project.ResumeWatchers();
		_onChange();
	}

	private void Uninstall()
	{
		_studioModel.Project.PauseWatchers();
		_projectPlugin!.RemoveFromProject();
		_studioModel.Project.ResumeWatchers();
		_onChange();
	}

	private void Downgrade()
	{
		_studioModel.Project.PauseWatchers();
		_collections.CopyDependenciesFromRepositoryToProject(_repositoryPlugin!, _settings);
		_repositoryPlugin!.CopyFromRepositoryToProject(_settings);
		_studioModel.Project.ResumeWatchers();
		_onChange();
	}

	private void CreateInRepository()
	{
		_studioModel.Project.PauseWatchers();
		_projectPlugin!.CopyFromProjectToRepository(_settings);
		_studioModel.Project.ResumeWatchers();
		_onChange();
	}
	#endregion
}