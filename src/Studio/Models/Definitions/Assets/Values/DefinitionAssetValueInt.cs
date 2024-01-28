using System.Reflection;
using Migration;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("ce302b13-d585-42d1-99bc-73b988e5807f")]
public sealed class DefinitionAssetValueInt : DefinitionAssetValue
{
	public override Type      InspectorControlType => typeof(InspectorIntControl);
	public override FieldInfo EditableField        => GetType().GetField(nameof(Value))!;
	public          int       Value;

	public override void WriteJsonValue(StringWriter stringWriter)
	{
		stringWriter.Write(Value);
	}

	public override void ImportFrom(DefinitionAssetValue other)
	{
		Value = other switch
		{
			DefinitionAssetValueBool otherBool => otherBool.Value ? 1 : 0,
			DefinitionAssetValueFloat otherFloat => (int)otherFloat.Value,
			DefinitionAssetValueInt otherInt => otherInt.Value,
			DefinitionAssetValueReference otherReference => 0,
			DefinitionAssetValueString otherString => int.TryParse(otherString.Value, out int result) ? result : 0,
			_ => throw new ArgumentOutOfRangeException(nameof(other))
		};
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValueInt))]
public interface IDefinitionAssetValueIntMigratable : IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueInt))]
public class DefinitionAssetValueInt_000 : DefinitionAssetValue_000, IDefinitionAssetValueIntMigratable
{
	public int Value;
}
#endregion