using Migration;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models.Project.Importers;

[Serializable, SerializedClassId("scene-importer-id")]
public sealed class SceneImporterSettings : ImporterSettings
{
	public override ProjectNodeType NodeType => ProjectNodeType.AssetMesh;

	[ReadOnlyInInspector] public List<SceneImporterMeshSettings> Meshes = new();
}

#region Migration
[Serializable, LatestVersion(typeof(SceneImporterSettings))]
public class SceneImporterSettings_000 : ImporterSettings_000
{
	public List<ISceneImporterMeshSettingsMigratable> Meshes;
}
#endregion