using System.Collections.Generic;
using System.IO;
using System.Xml;
using Mine.Framework;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

public sealed class PluginManagerSettings
{
    public const string StudioPluginsSettingsPath = "Plugins/PluginsSettings.xml";
    private const string ProjectPluginsPath        = "Plugins";

    private List<string> _repositoryPluginsPaths = new();
    private string?      _repositoryPath;
    private string?      _projectPath;
    private string?      _errorMessage;

    public string? RepositoryPath => _repositoryPath;
    public string? ProjectPath    => _projectPath;
    public string? ErrorMessage   => _errorMessage;
    public bool    IsError        => _errorMessage != null;

    public PluginManagerSettings(ProjectModel projectModel)
    {
        byte[]? settingsResource = Engine.Resources.ReadResource(StudioPluginsSettingsPath);
        if (settingsResource == null)
        {
            ConsoleViewModel.LogError($"Plugins setting file not found at: {StudioPluginsSettingsPath}");
            return;
        }

        ParseSettings(settingsResource);
        _repositoryPath = GetValidRepositoryPath(out _errorMessage);
        if (_errorMessage != null)
        {
            return;
        }

        _projectPath    = GetValidProjectPath(projectModel, out _errorMessage);
    }

    private string? GetValidRepositoryPath(out string? errorMessage)
    {
        if (_repositoryPluginsPaths.Count == 0)
        {
            errorMessage = "Repository paths are empty!";
            return null;
        }

        string? validPath = null;
        foreach (string path in _repositoryPluginsPaths)
        {
            if (Directory.Exists(path))
            {
                if (validPath == null)
                {
                    validPath = path;
                }
                else
                {
                    errorMessage = "Multiple repository paths are valid, cannot determine which one is correct!";
                    return null;
                }
            }
        }

        if (validPath == null)
        {
            errorMessage = "No valid repository path found. Add one!";
            return null;
        }

        errorMessage = null;
        return validPath;
    }

    private string? GetValidProjectPath(ProjectModel projectModel, out string? errorMessage)
    {
        if (projectModel.ProjectSettings == null)
        {
            errorMessage = "Project is not opened. Cannot evaluate project path!";
            return null;
        }

        string validPath = Path.Combine(projectModel.ProjectSettings.AbsoluteScriptsPath, ProjectPluginsPath);
        if (Directory.Exists(validPath))
        {
            errorMessage = null;
            return validPath;
        }

        errorMessage = $"Project path doesn't exist! ({validPath})";
        return null;
    }
    
    #region Initialization

    private void ParseSettings(byte[] settingsResource)
    {
        using Stream settingsStream = new MemoryStream(settingsResource);
        
        XmlDocument document       = new();
        try
        {
            document.Load(settingsStream);
        }
        catch (Exception e)
        {
            ConsoleViewModel.LogException($"Invalid plugins settings file at {StudioPluginsSettingsPath}, exception: {e}");
            return;
        }

        XmlNode? settingsNode = document.FindChild("settings");
        if (settingsNode == null)
        {
            ConsoleViewModel.LogError("Invalid plugins settings file structure. <settings> expected");
            return;
        }

        foreach (XmlNode child in settingsNode)
        {
            if (child.Name == "repository")
            {
                string? path = child.GetAttributeValueString("path");
                if (path != null)
                {
                    _repositoryPluginsPaths.Add(path);
                }
            }
        }
    }

    #endregion
}