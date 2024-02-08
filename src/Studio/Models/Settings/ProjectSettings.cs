using Migration;
using Mine.Framework;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models;

[Serializable, SerializedClassId("project-settings")]
public sealed class ProjectSettings
{
	// TODO - add template project settings json to template, so the namespace will be renamed to project name
	public string AssetsDatabaseSourcePath = "AssetsDatabase.cs";
	public string AssetsDatabaseNamespace  = "MyGame";
	
	[ReadOnlyInInspector, NonSerialized] public string ProjectFolderPath = "";

	[ReadOnlyInInspector, ShowInInspector, NonSerialized] private string? _relativeResourcesPath;
	public                  string  RelativeResourcesPath => _relativeResourcesPath ??= Engine.ResourcesPath;

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
}
#endregion