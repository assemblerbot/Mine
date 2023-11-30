using System;
using System.Collections.Generic;
using ImGuiNET;

namespace Mine.Studio;

public class PluginManagerFoldout
{
	private readonly PluginManagerSettings    _settings;
	private readonly PluginManagerCollections _collections;
	private          Action                   _onChange;

	private readonly PluginManagerPlugin? _repositoryPlugin;
	private readonly PluginManagerPlugin? _projectPlugin;
	private readonly PluginManagerPlugin  _plugin;
	
	private readonly string                _repositoryVersion;
	private readonly string                _projectVersion;
	private readonly string                _title;

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
		PluginManagerSettings settings,
		PluginManagerCollections              collections,
		PluginManagerCollections.CPluginsPair pluginsPair,
		Action onChange
	)
	{
		_settings    = settings;
		_collections = collections;
		_onChange    = onChange;

		// gather resources
		_repositoryPlugin = pluginsPair.RepositoryPlugin;
		_projectPlugin    = pluginsPair.ProjectPlugin;
		_plugin           = _projectPlugin ?? _repositoryPlugin!;

		_repositoryVersion = _repositoryPlugin?.Version ?? "not available";
		_projectVersion    = _projectPlugin?.Version    ?? "not installed";
		_title             = $"{_plugin.Name} ({_projectVersion})";

		_pluginsDependentOnThis = collections.GetAllProjectPluginsDependentOn(_plugin.Id);
		_isDependencyError      = collections.IsDependencyError(_plugin);
		_isVersionError         = (_repositoryPlugin?.ParseVersion.IsError ?? false) || (_projectPlugin?.ParseVersion.IsError ?? false);

		// status
		if (IsError)
		{
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
				SimplePluginManagerVersion repositoryVersionNumber = _repositoryPlugin.ParseVersion;
				SimplePluginManagerVersion projectVersionNumber    = _projectPlugin.ParseVersion;
				string                     apiBreak = SimplePluginManagerVersion.IsApiBreak(repositoryVersionNumber, projectVersionNumber) ? " INCOMPATIBLE" : "";

				if (projectVersionNumber < repositoryVersionNumber || projectVersionNumber == repositoryVersionNumber)
				{
					if (projectVersionNumber < repositoryVersionNumber)
					{
						_buttonNameIdUpgrade = $"Upgrade to{_repositoryPlugin.Version}{apiBreak}##{_plugin.Id}.upgrade";
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
		ImGui.Text("+"); // status icon - TODO
	}

	private void DescriptionUI()
	{
		ImGui.Text(_title);
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

		if (_textUpToDate != null)
		{
			ImGui.Text(_textUpToDate);
		}
	}

	private void UninstallationUI()
	{
		if (_buttonNameIdUninstall != null)
		{
			ImGui.Button(_buttonNameIdUninstall);
			return;
		}

		if (_buttonNameIdDowngrade != null)
		{
			ImGui.Button(_buttonNameIdDowngrade);
			return;
		}

		if (_textRequiredByOtherPlugins != null)
		{
			ImGui.Text(_textRequiredByOtherPlugins);
		}
	}
	#endregion

	#region Manipulation
	private void Install()
	{
		_collections.CopyDependenciesFromRepositoryToProject(_repositoryPlugin!, _settings);
		_repositoryPlugin!.CopyFromRepositoryToProject(_settings);
		_onChange();
	}

	private void Upgrade()
	{
		_collections.CopyDependenciesFromRepositoryToProject(_repositoryPlugin!, _settings);
		_repositoryPlugin!.CopyFromRepositoryToProject(_settings);
		_onChange();
	}

	private void CopyToRepository()
	{
		_projectPlugin!.CopyFromProjectToRepository(_settings);
		_onChange();
	}

	#endregion
	
/*
	public void OnGui(
		PluginManagerSettings                 settings,
		PluginManagerCollections              collections,
		PluginManagerCollections.CPluginsPair plugins_pair,
		PluginManagerStatusIcons              icons,
		Action                                on_collections_changed
	)
	{
		// gather resources
		PluginManagerPlugin repository_plugin = plugins_pair.RepositoryPlugin;
		PluginManagerPlugin project_plugin    = plugins_pair.ProjectPlugin;
		PluginManagerPlugin plugin            = project_plugin ?? repository_plugin;

		string repository_version = repository_plugin == null ? "not available" : repository_plugin.Version;
		string project_version    = project_plugin    == null ? "not installed" : project_plugin.Version;

		List<PluginManagerPlugin> plugins_dependent_on_this = collections.GetAllProjectPluginsDependentOn(plugin.Id);
		bool                      is_dependency_error       = collections.IsDependencyError(plugin);
		bool                      is_version_error          = (repository_plugin?.ParseVersion.IsError ?? false) || (project_plugin?.ParseVersion.IsError ?? false);
		bool                      is_error                  = is_dependency_error                                || is_version_error;

		// layout
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label(icons.GetPluginStatusIcon(repository_plugin, project_plugin), GUILayout.Width(20), GUILayout.Height(20));
				GUILayout.Label($"{plugin.Name} ({project_version})",                         EditorStyles.boldLabel);
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical(GUILayout.Width(768 -200));
			{
				// foldout
				EditorGUILayout.BeginHorizontal(GUILayout.Width(20));
				m_IsExpanded = EditorGUILayout.Foldout(m_IsExpanded, "") || is_error;
				if (!m_IsExpanded)
				{
					GuiButtons(settings, collections, repository_plugin, project_plugin, plugins_dependent_on_this.Count > 0, is_error, on_collections_changed);
				}
				EditorGUILayout.EndHorizontal();

				if (m_IsExpanded)
				{
					// description and info
					GUIStyle rich_text_style = new GUIStyle(GUI.skin.label);
					rich_text_style.richText = true;
					GUILayout.Label(plugin.Description, rich_text_style);

					GuiPluginDependencies(plugin, collections, icons);
					GuiPluginsDependentOn(plugins_dependent_on_this, collections, icons);
					GUILayout.Space(8);
					GUILayout.Label("Id:"          + plugin.Id);
					GUILayout.Label("Repository: " + repository_version);

					// errors
					if (repository_plugin != null && !string.IsNullOrEmpty(repository_plugin.Error))
					{
						SimplePluginManagerUIUtils.ErrorLabel(repository_plugin.Error);
					}

					if (project_plugin != null && !string.IsNullOrEmpty(project_plugin.Error))
					{
						SimplePluginManagerUIUtils.ErrorLabel(project_plugin.Error);
					}

					if (is_version_error)
					{
						SimplePluginManagerUIUtils.ErrorLabel("Plugin has invalid version format!");
					}

					if (is_dependency_error)
					{
						SimplePluginManagerUIUtils.ErrorLabel("Plugin is dependent on non existing plugin!");
					}

					// buttons
					if (
						(repository_plugin == null || string.IsNullOrEmpty(repository_plugin.Error))
						&&
						(project_plugin == null || string.IsNullOrEmpty(project_plugin.Error))
					)
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Label(""); //formatting
							GuiButtons(settings, collections, repository_plugin, project_plugin, plugins_dependent_on_this.Count > 0, is_error, on_collections_changed);
						}
						EditorGUILayout.EndHorizontal();
					}
				}
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();

		SimplePluginManagerUIUtils.Line();
	}

	private void GuiPluginDependencies(
		PluginManagerPlugin            plugin,
		SimplePluginManagerCollections collections,
		SimplePluginManagerStatusIcons icons
	)
	{
		if (plugin.Dependencies != null && plugin.Dependencies.Count > 0)
		{
			GUILayout.Space(8);
			GUILayout.Label("Required plugins:", EditorStyles.boldLabel);

			foreach (string dependency in plugin.Dependencies)
			{
				GuiPluginSublistItem(dependency, collections, icons);
			}
		}
	}

	private void GuiPluginsDependentOn(
		List<PluginManagerPlugin>      plugins_dependent_on_this,
		SimplePluginManagerCollections collections,
		SimplePluginManagerStatusIcons icons
	)
	{
		if (plugins_dependent_on_this.Count == 0)
		{
			return;
		}

		GUILayout.Space(8);
		GUILayout.Label("Required by plugins:", EditorStyles.boldLabel);
		foreach (PluginManagerPlugin plugin in plugins_dependent_on_this)
		{
			GuiPluginSublistItem(plugin.Id, collections, icons);
		}
	}

	private void GuiPluginSublistItem(
		string                         plugin_id,
		SimplePluginManagerCollections collections,
		SimplePluginManagerStatusIcons icons
	)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(16);
		GUILayout.Label(
			icons.GetDependencyStatusIcon(
				collections.IsInProject(plugin_id),
				collections.IsInRepository(plugin_id)
			),
			GUILayout.Width(20)
		);
		GUILayout.Label(collections.GetPluginNameAndVersion(plugin_id));
		GUILayout.EndHorizontal();
	}

	private void GuiButtons(
		SimplePluginManagerSettings    settings,
		SimplePluginManagerCollections collections,
		PluginManagerPlugin            repository_plugin,
		PluginManagerPlugin            project_plugin,
		bool                           any_plugins_dependent_on_this,
		bool                           is_error,
		Action                         on_collections_changed
	)
	{
		if (repository_plugin != null)
		{
			if (project_plugin == null)
			{
				if (!is_error && GUILayout.Button("Install", GUILayout.Width(256)))
				{
					collections.CopyDependenciesFromRepositoryToProject(repository_plugin, settings);
					repository_plugin.CopyFromRepositoryToProject(settings);
					on_collections_changed();
					AssetDatabase.Refresh();
				}

				GUILayout.Label(" ", GUILayout.Width(256));
			}
			else
			{
				SimplePluginManagerVersion repository_version_number = repository_plugin.ParseVersion;
				SimplePluginManagerVersion project_version_number    = project_plugin.ParseVersion;
				string api_break =
					SimplePluginManagerVersion.IsApiBreak(repository_version_number, project_version_number) ? " INCOMPATIBLE" : "";

				if (project_version_number < repository_version_number)
				{
					if (!is_error && GUILayout.Button($"Upgrade to {repository_plugin.Version}{api_break}", GUILayout.Width(256)))
					{
						collections.CopyDependenciesFromRepositoryToProject(repository_plugin, settings);
						repository_plugin.CopyFromRepositoryToProject(settings);
						on_collections_changed();
						AssetDatabase.Refresh();
					}

					if (any_plugins_dependent_on_this)
					{
						GUILayout.Label("Required by other plugin(s).", GUILayout.Width(256));
					}
					else if(GUILayout.Button("Uninstall", GUILayout.Width(256)))
					{
						project_plugin.RemoveFromProject();
						on_collections_changed();
						AssetDatabase.Refresh();
					}
				}
				else if (project_version_number == repository_version_number)
				{
					GUILayout.Label("Up to date.", EditorStyles.boldLabel, GUILayout.Width(256));

					if (any_plugins_dependent_on_this)
					{
						GUILayout.Label("Required by other plugin(s).", GUILayout.Width(256));
					}
					else if(GUILayout.Button("Uninstall", GUILayout.Width(256)))
					{
						project_plugin.RemoveFromProject();
						on_collections_changed();
						AssetDatabase.Refresh();
					}
				}
				else
				{
					if (!is_error && GUILayout.Button($"Downgrade to {repository_plugin.Version}{api_break}", GUILayout.Width(256)))
					{
						collections.CopyDependenciesFromRepositoryToProject(repository_plugin, settings);
						repository_plugin.CopyFromRepositoryToProject(settings);
						on_collections_changed();
						AssetDatabase.Refresh();
					}

					if (!is_error && GUILayout.Button("Copy to repository", GUILayout.Width(256)))
					{
						project_plugin.CopyFromProjectToRepository(settings);
						on_collections_changed();
						AssetDatabase.Refresh();
					}
				}
			}
		}
		else
		{
			if (project_plugin != null)
			{
				GUILayout.Label("", GUILayout.Width(256));

				if (!is_error && GUILayout.Button("Create in repository", GUILayout.Width(256)))
				{
					project_plugin.CopyFromProjectToRepository(settings);
					on_collections_changed();
					AssetDatabase.Refresh();
				}
			}
		}
	}
	*/	
}