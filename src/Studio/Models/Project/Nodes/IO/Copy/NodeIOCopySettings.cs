using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("d6112cef-1810-416d-b26b-9e9b75529b64")]
public sealed class NodeIOCopySettings : NodeIOSettings
{
}

#region Migration
[MigratableInterface(typeof(NodeIOCopySettings))]
public interface INodeIOCopySettingsMigratable : INodeIOSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOCopySettings))]
public class NodeIOCopySettings_000 : NodeIOSettings_000, INodeIOCopySettingsMigratable
{
}
#endregion