using System.Globalization;
using System.Reflection;
using Migration;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("7f82bf71-e5b8-4fe9-8e2a-8d86853aaf21")]
public sealed class DefinitionAssetValueString : DefinitionAssetValue
{
	public override Type      InspectorControlType => typeof(InspectorStringControl);
	public override FieldInfo EditableField        => GetType().GetField(nameof(Value))!;
	public          string    Value;

	public override void WriteJsonValue(StringWriter stringWriter)
	{
		stringWriter.Write('"');
		stringWriter.Write(Value);
		stringWriter.Write('"');
	}

	public override void ImportFrom(DefinitionAssetValue other)
	{
		Value = other switch
		{
			DefinitionAssetValueBool otherBool => otherBool.Value.ToString(),
			DefinitionAssetValueFloat otherFloat => otherFloat.Value.ToString(CultureInfo.InvariantCulture),
			DefinitionAssetValueInt otherInt => otherInt.Value.ToString(),
			DefinitionAssetValueReference otherReference => "",
			DefinitionAssetValueString otherString => otherString.Value,
			_ => throw new ArgumentOutOfRangeException(nameof(other))
		};
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValueString))]
public interface IDefinitionAssetValueStringMigratable : IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueString))]
public class DefinitionAssetValueString_000 : DefinitionAssetValue_000, IDefinitionAssetValueStringMigratable
{
	public string Value;
}
#endregion