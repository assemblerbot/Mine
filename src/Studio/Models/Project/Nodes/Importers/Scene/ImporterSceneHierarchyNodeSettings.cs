using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("4c7e4c73-e41f-4044-a733-ef7d6df38f41")]
public sealed class ImporterSceneHierarchyNodeSettings
{
	[ReadOnlyInInspector] public string                                  Name;
	[ReadOnlyInInspector] public List<ImporterSceneHierarchyNodeSettings>? Children  = null;
	[ReadOnlyInInspector] public List<int>                               Meshes    = new();
	[ReadOnlyInInspector] public List<int>                               Materials = new();

	public ImporterSceneHierarchyNodeSettings(string name)
	{
		Name = name;
	}
}

#region Migration

[MigratableInterface(typeof(ImporterSceneHierarchyNodeSettings))]
public interface IImporterSceneHierarchyNodeSettingsMigratable;

[Serializable, LatestVersion(typeof(ImporterSceneHierarchyNodeSettings))]
public class ImporterSceneHierarchyNodeSettings_000 : IImporterSceneHierarchyNodeSettingsMigratable
{
	public string Name;
	
	[MigrateField] public List<IImporterSceneHierarchyNodeSettingsMigratable>? Children;
	public                List<int>                                          Meshes;
	public                List<int>                                          Materials;
}
#endregion