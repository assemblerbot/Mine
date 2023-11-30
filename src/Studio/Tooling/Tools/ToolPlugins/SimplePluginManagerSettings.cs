using System.Collections.Generic;
using System.IO;
using RedHerring.Studio.Models.Project;

namespace Mine.Studio;

public sealed class SimplePluginManagerSettings
{
    private List<string> RepositoryPluginsPaths;
    private string       ProjectPluginsPath = "Plugins";

    public string GetValidRepositoryPath(out string error_message)
    {
        if (RepositoryPluginsPaths == null || RepositoryPluginsPaths.Count == 0)
        {
            error_message = "Repository paths are empty!";
            return null;
        }

        string valid_path = null;
        foreach (string path in RepositoryPluginsPaths)
        {
            if (Directory.Exists(path))
            {
                if (valid_path == null)
                {
                    valid_path = path;
                }
                else
                {
                    error_message = "Multiple repository paths are valid, cannot determine which one is correct!";
                    return null;
                }
            }
        }

        if (valid_path == null)
        {
            error_message = "No valid repository path found. Add one!";
            return null;
        }

        error_message = null;
        return valid_path;
    }

    public string GetValidProjectPath(ProjectModel projectModel, out string error_message)
    {
        string valid_path = Path.Combine(projectModel.ProjectSettings.AbsoluteResourcesPath, ProjectPluginsPath);
        if (Directory.Exists(valid_path))
        {
            error_message = null;
            return valid_path;
        }

        error_message = $"Project path doesn't exist! ({valid_path})";
        return null;
    }
}