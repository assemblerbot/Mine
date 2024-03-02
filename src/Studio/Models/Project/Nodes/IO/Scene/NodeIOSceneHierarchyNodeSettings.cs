using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("4c7e4c73-e41f-4044-a733-ef7d6df38f41")]
public sealed class NodeIOSceneHierarchyNodeSettings
{
	[ReadOnlyInInspector] public string                                  Name;
	[ReadOnlyInInspector] public List<NodeIOSceneHierarchyNodeSettings>? Children  = null;
	[ReadOnlyInInspector] public List<int>                               Meshes    = new();
	[ReadOnlyInInspector] public List<int>                               Materials = new();

	public NodeIOSceneHierarchyNodeSettings(string name)
	{
		Name = name;
	}
}

#region Migration

[MigratableInterface(typeof(NodeIOSceneHierarchyNodeSettings))]
public interface INodeIOSceneHierarchyNodeSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOSceneHierarchyNodeSettings))]
public class NodeIOSceneHierarchyNodeSettings_000 : INodeIOSceneHierarchyNodeSettingsMigratable
{
	public string Name;
	
	[MigrateField] public List<INodeIOSceneHierarchyNodeSettingsMigratable>? Children;
	public                List<int>                                          Meshes;
	public                List<int>                                          Materials;
}
#endregion