using System.Reflection;
using Migration;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("d4709c62-3053-4f8c-ad43-2a29ac9facc2")]
public sealed class DefinitionAssetValueBool : DefinitionAssetValue
{
	public override Type      InspectorControlType => typeof(InspectorBoolControl);
	public override FieldInfo EditableField        => GetType().GetField(nameof(Value))!;
	public          bool      Value;

	public override void WriteJsonValue(StringWriter stringWriter)
	{
		stringWriter.Write(Value ? "true" : "false");
	}

	public override void ImportFrom(DefinitionAssetValue other)
	{
		Value = other switch
		{
			DefinitionAssetValueBool otherBool => otherBool.Value,
			DefinitionAssetValueFloat otherFloat => otherFloat.Value != 0,
			DefinitionAssetValueInt otherInt => otherInt.Value != 0,
			DefinitionAssetValueReference otherReference => false,
			DefinitionAssetValueString otherString => !string.IsNullOrEmpty(otherString.Value),
			_ => throw new ArgumentOutOfRangeException(nameof(other))
		};
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