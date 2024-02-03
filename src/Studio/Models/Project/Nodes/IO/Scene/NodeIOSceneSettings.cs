using Migration;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("ea019557-3078-4cf2-8bb1-f3ffe8d84e73")]
public sealed class NodeIOSceneSettings : NodeIOSettings
{
	public float NormalSmoothingAngle = 15f;
	
	[ReadOnlyInInspector] public List<NodeIOMeshSettings> Meshes = new();
}

#region Migration
[MigratableInterface(typeof(NodeIOSceneSettings))]
public interface INodeIOSceneSettingsMigratable : INodeIOSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOSceneSettings))]
public class NodeIOSceneSettings_000 : NodeIOSettings_000, INodeIOSceneSettingsMigratable
{
	public float SmoothingAngle;
	
	public List<INodeIOMeshSettingsMigratable> Meshes;
}
#endregion