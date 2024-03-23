using Migration;
using Mine.Studio;

namespace Mine.Studio;

[Serializable, SerializedClassId("ea019557-3078-4cf2-8bb1-f3ffe8d84e73")]
public sealed class ImporterSceneSettings : ImporterSettings
{
	public float NormalSmoothingAngle = 15f;
	public bool  CompensateFBXScale   = false;
	
	[ReadOnlyInInspector] public List<ImporterSceneMeshSettings> Meshes = new();

	[ReadOnlyInInspector] public List<ImporterSceneMaterialSettings> Materials = new();

	[ReadOnlyInInspector] public ImporterSceneHierarchyNodeSettings Root = new("Root");
}

#region Migration
[MigratableInterface(typeof(ImporterSceneSettings))]
public interface IImporterSceneSettingsMigratable : IImporterSettingsMigratable;

[Serializable, LatestVersion(typeof(ImporterSceneSettings))]
public class ImporterSceneSettings_000 : ImporterSettings_000, IImporterSceneSettingsMigratable
{
	public float SmoothingAngle;
	public bool  CompensateFBXScale;
	
	[MigrateField] public List<IImporterSceneMeshSettingsMigratable> Meshes;

	[MigrateField] public List<IImporterSceneMaterialSettingsMigratable> Materials;

	public IImporterSceneHierarchyNodeSettingsMigratable Root;
}
#endregion