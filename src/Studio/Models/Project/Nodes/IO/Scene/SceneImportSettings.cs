using Migration;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("ea019557-3078-4cf2-8bb1-f3ffe8d84e73")]
public sealed class SceneImportSettings : ImportSettings
{
	[ReadOnlyInInspector] public List<SceneImportMeshSettings> Meshes = new();
}

#region Migration
[MigratableInterface(typeof(SceneImportSettings))]
public interface ISceneImportSettingsMigratable : IImportSettingsMigratable;

[Serializable, LatestVersion(typeof(SceneImportSettings))]
public class SceneImportSettings_000 : ImportSettings_000, ISceneImportSettingsMigratable
{
	public List<ISceneImportMeshSettingsMigratable> Meshes;
}
#endregion