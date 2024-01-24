using Migration;

namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("7d81811d-0a7a-4e75-86b8-c99d389950b3")]
public sealed class NodeIOAssetDefinitionSettings : NodeIOSettings
{
}

#region Migration
[MigratableInterface(typeof(NodeIOAssetDefinitionSettings))]
public interface INodeIOAssetDefinitionSettingsMigratable : INodeIOSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOAssetDefinitionSettings))]
public class NodeIOAssetDefinitionSettings_000 : NodeIOSettings_000, INodeIOAssetDefinitionSettingsMigratable
{
}
#endregion