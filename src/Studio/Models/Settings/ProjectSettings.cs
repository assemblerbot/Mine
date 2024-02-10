using Migration;
using Mine.Framework;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models;

[Serializable, SerializedClassId("project-settings")]
public sealed class ProjectSettings
{
	public string AssetDatabaseSourcePath = "AssetDatabase.cs";
	public string AssetDatabaseNamespace  = "MyGame";
	public string AssetDatabaseClass      = "AssetDatabase";
	
	[ReadOnlyInInspector, NonSerialized] public string ProjectFolderPath = "";

	[ReadOnlyInInspector, ShowInInspector, NonSerialized] private string? _relativeResourcesPath;
	public                  string  RelativeResourcesPath => _relativeResourcesPath ??= Resources.RootPath;

	[ReadOnlyInInspector, ShowInInspector, NonSerialized] private string? _absoluteResourcesPath;
	public                  string  AbsoluteResourcesPath => _absoluteResourcesPath ??= Path.Combine(ProjectFolderPath, RelativeResourcesPath);

	[ReadOnlyInInspector, ShowInInspector, NonSerialized] private string? _absoluteScriptsPath;
	public                  string  AbsoluteScriptsPath => _absoluteScriptsPath ??= Path.Combine(ProjectFolderPath, "GameLibrary");

	[ReadOnlyInInspector, ShowInInspector, NonSerialized] private string? _absoluteAssetsPath;
	public                  string  AbsoluteAssetsPath => _absoluteAssetsPath ??= Path.Combine(ProjectFolderPath, "Assets");
}

#region Migration
[MigratableInterface(typeof(ProjectSettings))]
public interface IProjectSettingsMigratable
{
}
    
[Serializable, LatestVersion(typeof(ProjectSettings))]
public class ProjectSettings_000 : IProjectSettingsMigratable
{
	public string AssetDatabaseSourcePath = null!;
	public string AssetDatabaseNamespace = null!;
	public string AssetDatabaseClass = null!;
}
#endregion