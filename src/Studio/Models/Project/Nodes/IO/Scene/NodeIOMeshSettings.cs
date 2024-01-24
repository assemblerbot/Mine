using Migration;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("d86709f5-c520-4c47-b4da-8278fff56eb6")]
public sealed class NodeIOMeshSettings
{
	[ReadOnlyInInspector] public string Name;
	public bool Import = true;

	public NodeIOMeshSettings(string name)
	{
		Name = name;
	}
}

#region Migration

[MigratableInterface(typeof(NodeIOMeshSettings))]
public interface INodeIOMeshSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOMeshSettings))]
public class NodeIOMeshSettings_000 : INodeIOMeshSettingsMigratable
{
	public string Name;
	public bool Import = true;
}
#endregion