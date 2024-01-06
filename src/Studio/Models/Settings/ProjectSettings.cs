using Migration;
using Mine.Framework;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models;

[Serializable, SerializedClassId("project-settings")]
public sealed class ProjectSettings
{
	[ReadOnlyInInspector, NonSerialized] public string ProjectFolderPath = "";

	[NonSerialized] private string? _relativeResourcesPath;
	public                  string  RelativeResourcesPath => _relativeResourcesPath ??= Engine.ResourcesPath;

	[NonSerialized] private string? _absoluteResourcesPath;
	public                  string  AbsoluteResourcesPath => _absoluteResourcesPath ??= Path.Combine(ProjectFolderPath, RelativeResourcesPath);

	[NonSerialized] private string? _absoluteScriptsPath;
	public                  string  AbsoluteScriptsPath => _absoluteScriptsPath ??= Path.Combine(ProjectFolderPath, "GameLibrary");

	[NonSerialized] private string? _absoluteAssetsPath;
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