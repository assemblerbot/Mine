namespace Mine.Studio;

[Serializable]
public class SimplePluginManagerPlugin
{
    // JSON serialized data
    public string       Id;
    public string       Name;
    public string       Description;
    public string       Version;
    public List<string> Dependencies;
    public List<string> GlobalDefines;
        
    // non serialized data
    [NonSerialized] public string PathToPlugin;
    [NonSerialized] public string Error;
        
    public SimplePluginManagerVersion ParseVersion => new SimplePluginManagerVersion(Version);

    public void CopyFromRepositoryToProject(SimplePluginManagerSettings settings)
    {
        SimplePluginManagerFileUtils.CopyPlugin(
            PathToPlugin,
            settings.GetValidRepositoryPath(out string repository_error_message),
            settings.GetValidProjectPath(out string project_error_message)
        );
        AddGlobalDefines();
    }

    public void CopyFromProjectToRepository(SimplePluginManagerSettings settings)
    {
        SimplePluginManagerFileUtils.CopyPlugin(
            PathToPlugin,
            settings.GetValidProjectPath(out string project_error_message),
            settings.GetValidRepositoryPath(out string repository_error_message)
        );
    }

    public void RemoveFromProject()
    {
        SimplePluginManagerFileUtils.DeletePlugin(PathToPlugin);
        RemoveGlobalDefines();
    }

    private void AddGlobalDefines()
    {
        foreach (string global_define in GlobalDefines)
        {
            SimplePluginManagerUnityUtils.AddGlobalDefine(global_define);
        }
    }
        
    private void RemoveGlobalDefines()
    {
        foreach (string global_define in GlobalDefines)
        {
            SimplePluginManagerUnityUtils.RemoveGlobalDefine(global_define);
        }
    }
}