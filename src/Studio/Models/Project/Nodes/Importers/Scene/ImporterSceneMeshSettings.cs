using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("d86709f5-c520-4c47-b4da-8278fff56eb6")]
public sealed class ImporterSceneMeshSettings
{
	[ReadOnlyInInspector] public string Name;
	[ReadOnlyInInspector] public int    MaterialIndex;
	public                       bool   Import = true;

	public ImporterSceneMeshSettings(string name, int materialIndex)
	{
		Name          = name;
		MaterialIndex = materialIndex;
	}
}

#region Migration

[MigratableInterface(typeof(ImporterSceneMeshSettings))]
public interface IImporterSceneMeshSettingsMigratable;

[Serializable, LatestVersion(typeof(ImporterSceneMeshSettings))]
public class ImporterSceneMeshSettings_000 : IImporterSceneMeshSettingsMigratable
{
	public string Name;
	public int    MaterialIndex;
	public bool   Import = true;
}
#endregion