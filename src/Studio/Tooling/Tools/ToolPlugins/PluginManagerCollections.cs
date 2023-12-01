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

	private PluginManagerCollection _repository = null!;
	private PluginManagerCollection _project = null!;

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

		// repository
		_repository  = new PluginManagerCollection();
		_errorMessage = _repository.Scan(settings.RepositoryPath!, false);
		if (_errorMessage != null)
		{
			return;
		}

		// project - scripts
		_project     = new PluginManagerCollection();
		_errorMessage = _project.Scan(settings.ProjectScriptsPath!, false);
		if (_errorMessage != null)
		{
			return;
		}

		// project - assets
		_errorMessage = _project.Scan(settings.ProjectAssetsPath!, true);
	}
        
	public List<CPluginsPair> BuildListOfPlugins()
	{
		List<CPluginsPair> listOfPlugins = new ();

		foreach (PluginManagerPlugin repositoryPlugin in _repository.Plugins.Values)
		{
			PluginManagerPlugin? projectPlugin = null;
			_project.Plugins?.TryGetValue(repositoryPlugin.Id, out projectPlugin);
			listOfPlugins.Add(new CPluginsPair {RepositoryPlugin = repositoryPlugin, ProjectPlugin = projectPlugin});
		}

		foreach (PluginManagerPlugin projectPlugin in _project.Plugins!.Values)
		{
			PluginManagerPlugin? repositoryPlugin = null;
			_repository.Plugins?.TryGetValue(projectPlugin.Id, out repositoryPlugin);

			if (repositoryPlugin == null)
			{
				// this plugin exists in project only
				listOfPlugins.Add(new CPluginsPair {RepositoryPlugin = null, ProjectPlugin = projectPlugin});
			}
		}

		listOfPlugins.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCulture));
		return listOfPlugins;
	}

	public bool IsAnyProjectPluginDependentOn(string pluginId)
	{
		return _project.Plugins.Any(x => x.Value.Dependencies != null && x.Value.Dependencies.Contains(pluginId));
	}

	public List<PluginManagerPlugin> GetAllProjectPluginsDependentOn(string pluginId)
	{
		return _project.Plugins.Values.ToList().FindAll(x => x.Dependencies != null && x.Dependencies.Contains(pluginId));
	}

	public bool IsInProject(string pluginId)
	{
		return _project.Plugins.ContainsKey(pluginId);
	}

	public bool IsInRepository(string pluginId)
	{
		return _repository.Plugins.ContainsKey(pluginId);
	}

	public bool IsDependencyError(PluginManagerPlugin plugin, bool dependenciesMustBeInstalled)
	{
		foreach (string dependency in plugin.Dependencies)
		{
			if (_project.Plugins.ContainsKey(dependency))
			{
				continue;
			}

			if (dependenciesMustBeInstalled)
			{
				return true;
			}

			if(_repository.Plugins.ContainsKey(dependency))
			{
				continue;
			}

			return true;
		}

		return false;
	}
	
	public string GetPluginName(string pluginId)
	{
		if (IsInProject(pluginId))
		{
			return _project.Plugins[pluginId].Name;
		}

		if (IsInRepository(pluginId))
		{
			return _repository.Plugins[pluginId].Name;
		}

		return pluginId;
	}

	public string GetPluginVersion(string pluginId)
	{
		if (IsInProject(pluginId))
		{
			return _project.Plugins[pluginId].Version;
		}

		if (IsInRepository(pluginId))
		{
			return _repository.Plugins[pluginId].Version;
		}

		return "";
	}
	
	public void CopyDependenciesFromRepositoryToProject(PluginManagerPlugin plugin, PluginManagerSettings settings)
	{
		List<string> installedDependencies = new List<string>();
	        
		foreach (string dependency in plugin.Dependencies)
		{
			if (IsInProject(dependency))
			{
				continue; // already installed
			}

			if (IsInRepository(dependency))
			{
				// can be installed
				_repository.Plugins[dependency].CopyFromRepositoryToProject(settings);
				installedDependencies.Add(dependency);
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
		foreach (string installedDependency in installedDependencies)
		{
			CopyDependenciesFromRepositoryToProject(_repository.Plugins[installedDependency], settings);
		}
	}
}