using Migration;
using Mine.Studio;

namespace Mine.Studio;

[Serializable, SerializedClassId("d86709f5-c520-4c47-b4da-8278fff56eb6")]
public sealed class NodeIOSceneMeshSettings
{
	[ReadOnlyInInspector] public string Name;
	public bool Import = true;

	public NodeIOSceneMeshSettings(string name)
	{
		Name = name;
	}
}

#region Migration

[MigratableInterface(typeof(NodeIOSceneMeshSettings))]
public interface INodeIOSceneMeshSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOSceneMeshSettings))]
public class NodeIoSceneMeshSettings_000 : INodeIOSceneMeshSettingsMigratable
{
	public string Name;
	public bool Import = true;
}
#endregion