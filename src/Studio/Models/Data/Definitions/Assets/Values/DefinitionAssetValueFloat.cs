using System.Globalization;
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

	public override void ImportFrom(DefinitionAssetValue other)
	{
		Value = other switch
		{
			DefinitionAssetValueBool otherBool => otherBool.Value ? 1f : 0f,
			DefinitionAssetValueFloat otherFloat => otherFloat.Value,
			DefinitionAssetValueInt otherInt => otherInt.Value,
			DefinitionAssetValueReference otherReference => 0f,
			DefinitionAssetValueString otherString => float.TryParse(otherString.Value, CultureInfo.InvariantCulture, out float result) ? result : 0f,
			_ => throw new ArgumentOutOfRangeException(nameof(other))
		};
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