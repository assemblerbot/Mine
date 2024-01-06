using Migration;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("d4709c62-3053-4f8c-ad43-2a29ac9facc2")]
public sealed class DefinitionAssetValueBool : DefinitionAssetValue
{
	public override Type InspectorControlType => typeof(InspectorBoolControl);
	public          bool Value;

	public override void WriteJsonValue(StringWriter stringWriter)
	{
		stringWriter.Write(Value ? "true" : "false");
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValueBool))]
public interface IDefinitionAssetValueBoolMigratable : IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueBool))]
public class DefinitionAssetValueBool_000 : DefinitionAssetValue_000, IDefinitionAssetValueBoolMigratable
{
	public bool Value;
}
#endregion