namespace Mine.Studio;

[Serializable]
public class PluginManagerPlugin
{
    //private const string _targetScripts = "Scripts"; // everything which is not Assets is Scripts
    private const string _targetAssets = "Assets";
    
    // JSON serialized data
    public string        Id;
    public string        Name;
    public string        Description;
    public string        Version;
    public string?       Target;
    public List<string>? Dependencies;
    public List<string>? GlobalDefines;
        
    // non serialized data
    [NonSerialized] public string  PathToPlugin = null!;
    [NonSerialized] public string? Error;
    
    public bool IsAssetPlugin => Target == _targetAssets;
        
    public PluginManagerVersion ParseVersion => new PluginManagerVersion(Version);

    public void CopyFromRepositoryToProject(PluginManagerSettings settings)
    {
        PluginManagerFileUtils.CopyPlugin(
            PathToPlugin,
            settings.RepositoryPath!,
            IsAssetPlugin ? settings.ProjectAssetsPath! : settings.ProjectScriptsPath!
        );
        AddGlobalDefines();
    }

    public void CopyFromProjectToRepository(PluginManagerSettings settings)
    {
        PluginManagerFileUtils.CopyPlugin(
            PathToPlugin,
            IsAssetPlugin ? settings.ProjectAssetsPath! : settings.ProjectScriptsPath!,
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