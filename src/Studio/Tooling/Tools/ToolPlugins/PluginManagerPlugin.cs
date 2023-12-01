namespace Mine.Studio;

[Serializable]
public class PluginManagerPlugin
{
    // JSON serialized data
    public string        Id;
    public string        Name;
    public string        Description;
    public string        Version;
    public List<string>? Dependencies;
    public List<string>? GlobalDefines;
        
    // non serialized data
    [NonSerialized] public string  PathToPlugin = null!;
    [NonSerialized] public string? Error;
        
    public PluginManagerVersion ParseVersion => new PluginManagerVersion(Version);

    public void CopyFromRepositoryToProject(PluginManagerSettings settings)
    {
        PluginManagerFileUtils.CopyPlugin(
            PathToPlugin,
            settings.RepositoryPath!,
            settings.ProjectPath!
        );
        AddGlobalDefines();
    }

    public void CopyFromProjectToRepository(PluginManagerSettings settings)
    {
        PluginManagerFileUtils.CopyPlugin(
            PathToPlugin,
            settings.ProjectPath!,
            settings.RepositoryPath!
        );
    }

    public void RemoveFromProject()
    {
        PluginManagerFileUtils.DeletePlugin(PathToPlugin);
        RemoveGlobalDefines();
    }

    private void AddGlobalDefines()
    {
        if (GlobalDefines == null)
        {
            return;
        }

        foreach (string global_define in GlobalDefines)
        {
            PluginManagerUnityUtils.AddGlobalDefine(global_define);
        }
    }
        
    private void RemoveGlobalDefines()
    {
        if (GlobalDefines == null)
        {
            return;
        }

        foreach (string global_define in GlobalDefines)
        {
            PluginManagerUnityUtils.RemoveGlobalDefine(global_define);
        }
    }
}