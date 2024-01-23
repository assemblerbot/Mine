using Migration;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("d86709f5-c520-4c47-b4da-8278fff56eb6")]
public sealed class SceneImportMeshSettings
{
	[ReadOnlyInInspector] public string Name;
	public bool Import = true;

	public SceneImportMeshSettings(string name)
	{
		Name = name;
	}
}

#region Migration

[MigratableInterface(typeof(SceneImportMeshSettings))]
public interface ISceneImportMeshSettingsMigratable;

[Serializable, LatestVersion(typeof(SceneImportMeshSettings))]
public class SceneImportMeshSettings_000 : ISceneImportMeshSettingsMigratable
{
	public string Name;
	public bool Import = true;
}
#endregion