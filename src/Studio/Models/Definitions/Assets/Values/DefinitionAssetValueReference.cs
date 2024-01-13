using System.Reflection;
using Migration;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("6df13d2c-9faa-4f48-b230-32348a4e0638")]
public class DefinitionAssetValueReference : DefinitionAssetValue
{
	public override Type            InspectorControlType => typeof(InspectorReferenceControl);
	public override FieldInfo       EditableField        => GetType().GetField(nameof(Value))!;
	public          StudioReference Value;

	public DefinitionAssetValueReference(StudioReference value)
	{
		Value = value;
	}

	public override void WriteJsonValue(StringWriter stringWriter)
	{
		// TODO
		//stringWriter.Write(Value);
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValueReference))]
public interface IDefinitionAssetValueReferenceMigratable : IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueReference))]
public class DefinitionAssetValueReference_000 : DefinitionAssetValue_000, IDefinitionAssetValueReferenceMigratable
{
	public int Value;
}
#endregion