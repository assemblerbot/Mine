using Migration;
using Mine.Studio;

namespace Mine.Studio;

[Serializable, SerializedClassId("8589a74d-43fc-4c5d-8537-87681debc89e")]
public sealed class NodeIOSceneMaterialSettings
{
	[ReadOnlyInInspector] public string Name;

	public NodeIOSceneMaterialSettings(string name)
	{
		Name = name;
	}
}

#region Migration

[MigratableInterface(typeof(NodeIOSceneMaterialSettings))]
public interface INodeIOSceneMaterialSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOSceneMaterialSettings))]
public class NodeIOSceneMaterialSettings_000 : INodeIOSceneMaterialSettingsMigratable
{
	public string Name;
}
#endregion