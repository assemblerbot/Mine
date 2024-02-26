using Migration;
using Mine.Studio;

namespace Mine.Studio;

[Serializable, SerializedClassId("ea019557-3078-4cf2-8bb1-f3ffe8d84e73")]
public sealed class NodeIOSceneSettings : NodeIOSettings
{
	public float NormalSmoothingAngle = 15f;
	
	[ReadOnlyInInspector] public List<NodeIOSceneMeshSettings> Meshes = new();

	[ReadOnlyInInspector] public List<NodeIOSceneMaterialSettings> Materials = new();

	[ReadOnlyInInspector] public NodeIOSceneHierarchyNodeSettings Root = new("Root");
}

#region Migration
[MigratableInterface(typeof(NodeIOSceneSettings))]
public interface INodeIOSceneSettingsMigratable : INodeIOSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOSceneSettings))]
public class NodeIOSceneSettings_000 : NodeIOSettings_000, INodeIOSceneSettingsMigratable
{
	public float SmoothingAngle;
	
	[MigrateField] public List<INodeIOSceneMeshSettingsMigratable> Meshes;

	[MigrateField] public List<INodeIOSceneMaterialSettingsMigratable> Materials;

	public INodeIOSceneHierarchyNodeSettingsMigratable Root;
}
#endregion