using Migration;
namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("1fbf8638-5ae8-49cd-be7c-846bbadd6951")]
public class NodeIOSettings
{
}

#region Migration
[MigratableInterface(typeof(NodeIOSettings))]
public interface INodeIOSettingsMigratable
{
}
    
[Serializable, LatestVersion(typeof(NodeIOSettings))]
public class NodeIOSettings_000 : INodeIOSettingsMigratable
{
}
#endregion