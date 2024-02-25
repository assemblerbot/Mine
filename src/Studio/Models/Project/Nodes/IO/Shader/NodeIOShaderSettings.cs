using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("733fc598-ff87-41c5-8f11-997f25d715a5")]
public sealed class NodeIOShaderSettings : NodeIOSettings
{
	public string            EntryPoint  = "main";
	public NodeIOShaderStage ShaderStage = NodeIOShaderStage.vertex;
}

#region Migration
[MigratableInterface(typeof(NodeIOShaderSettings))]
public interface INodeIOShaderSettingsMigratable : INodeIOSettingsMigratable;

[Serializable, LatestVersion(typeof(NodeIOShaderSettings))]
public class NodeIOShaderSettings_000 : NodeIOSettings_000, INodeIOShaderSettingsMigratable
{
	public string            EntryPoint;
	public NodeIOShaderStage ShaderStage;
}
#endregion