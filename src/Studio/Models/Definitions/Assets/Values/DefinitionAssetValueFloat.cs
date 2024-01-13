using System.Reflection;
using Migration;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("2bd5066a-c45a-4585-93b9-0b481bfe1b64")]
public sealed class DefinitionAssetValueFloat : DefinitionAssetValue
{
	public override Type      InspectorControlType => typeof(InspectorFloatControl);
	public override FieldInfo EditableField        => GetType().GetField(nameof(Value))!;
	public          float     Value;

	public override void WriteJsonValue(StringWriter stringWriter)
	{
		stringWriter.Write(Value);
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValueFloat))]
public interface IDefinitionAssetValueFloatMigratable : IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueFloat))]
public class DefinitionAssetValueFloat_000 : DefinitionAssetValue_000, IDefinitionAssetValueFloatMigratable
{
	public float Value;
}
#endregion