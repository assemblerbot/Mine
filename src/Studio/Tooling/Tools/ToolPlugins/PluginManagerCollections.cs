using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

public class PluginManagerCollections
{
	public class CPluginsPair
	{
		public PluginManagerPlugin? RepositoryPlugin;
		public PluginManagerPlugin? ProjectPlugin;

		public string Id   => RepositoryPlugin?.Id   ?? ProjectPlugin!.Id;
		public string Name => RepositoryPlugin?.Name ?? ProjectPlugin!.Name;
	}

	private SimplePluginManagerCollection _repository = null!;
	private SimplePluginManagerCollection _project = null!;

	private string? _errorMessage;
	public  bool    IsError      => _errorMessage != null;
	public  string? ErrorMessage => _errorMessage;
        
	public void Refresh(PluginManagerSettings settings)
	{
		if (settings.IsError)
		{
			_errorMessage = "Plugins not initialized!";
			return;
		}

		_repository  = new SimplePluginManagerCollection();
		_errorMessage = _repository.Init(settings.RepositoryPath!);
		if (_errorMessage != null)
		{
			return;
		}

		_project     = new SimplePluginManagerCollection();
		_errorMessage = _project.Init(settings.ProjectPath!);
	}
        
	public List<CPluginsPair> BuildListOfPlugins()
	{
		List<CPluginsPair> list_of_plugins = new ();

		foreach (PluginManagerPlugin repository_plugin in _repository.Plugins.Values)
		{
			PluginManagerPlugin project_plugin = null;
			_project.Plugins?.TryGetValue(repository_plugin.Id, out project_plugin);
			list_of_plugins.Add(new CPluginsPair {RepositoryPlugin = repository_plugin, ProjectPlugin = project_plugin});
		}

		foreach (PluginManagerPlugin project_plugin in _project.Plugins.Values)
		{
			PluginManagerPlugin repository_plugin = null;
			_repository.Plugins?.TryGetValue(project_plugin.Id, out repository_plugin);

			if (repository_plugin == null)
			{
				// this plugin exists in project only
				list_of_plugins.Add(new CPluginsPair {RepositoryPlugin = null, ProjectPlugin = project_plugin});
			}
		}

		list_of_plugins.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCulture));
		return list_of_plugins;
	}

	public bool IsAnyProjectPluginDependentOn(string plugin_id)
	{
		return _project.Plugins.Any(x => x.Value.Dependencies != null && x.Value.Dependencies.Contains(plugin_id));
	}

	public List<PluginManagerPlugin> GetAllProjectPluginsDependentOn(string plugin_id)
	{
		return _project.Plugins.Values.ToList().FindAll(x => x.Dependencies != null && x.Dependencies.Contains(plugin_id));
	}

	public bool IsInProject(string plugin_id)
	{
		return _project.Plugins.ContainsKey(plugin_id);
	}

	public bool IsInRepository(string plugin_id)
	{
		return _repository.Plugins.ContainsKey(plugin_id);
	}

	public bool IsDependencyError(PluginManagerPlugin plugin)
	{
		foreach (string dependency in plugin.Dependencies)
		{
			if (_project.Plugins.ContainsKey(dependency))
			{
				continue;
			}
                
			if(_repository.Plugins.ContainsKey(dependency))
			{
				continue;
			}

			return true;
		}

		return false;
	}

	public string GetPluginNameAndVersion(string plugin_id)
	{
		if (IsInProject(plugin_id))
		{
			return $"{_project.Plugins[plugin_id].Name} ({_project.Plugins[plugin_id].Version})";
		}

		if (IsInRepository(plugin_id))
		{
			return $"{_repository.Plugins[plugin_id].Name} ({_repository.Plugins[plugin_id].Version})";
		}

		return plugin_id;
	}

	public void CopyDependenciesFromRepositoryToProject(PluginManagerPlugin plugin, PluginManagerSettings settings)
	{
		List<string> installed_dependencies = new List<string>();
	        
		foreach (string dependency in plugin.Dependencies)
		{
			if (IsInProject(dependency))
			{
				continue; // valid
			}

			if (IsInRepository(dependency))
			{
				// can be installed
				_repository.Plugins[dependency].CopyFromRepositoryToProject(settings);
				installed_dependencies.Add(dependency);
			}
		}

		// refresh arrays of plugins
		Refresh(settings);
		if (IsError)
		{
			ConsoleViewModel.Log($"Dependencies were NOT copied due to following error: {_errorMessage}", ConsoleItemType.Error);
			return;
		}

		// recursively install dependencies of new installed dependencies
		foreach (string installed_dependency in installed_dependencies)
		{
			CopyDependenciesFromRepositoryToProject(_repository.Plugins[installed_dependency], settings);
		}
	}
}