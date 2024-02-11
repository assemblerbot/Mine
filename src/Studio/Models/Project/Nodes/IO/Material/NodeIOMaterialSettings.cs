using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("1423f1c3-fb3c-4b40-b1bb-363909e700bc")]
public sealed class NodeIOMaterialSettings : NodeIOSettings
{
}

#region Migration

[MigratableInterface(typeof(NodeIOMaterialSettings))]
public interface INodeIOMaterialSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOMaterialSettings))]
public class NodeIOMaterialSettings_000 : INodeIOMaterialSettingsMigratable
{
}
#endregion