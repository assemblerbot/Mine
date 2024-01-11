using Migration;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("f82d86eb-7957-4c24-8b8d-3a1e04c86ec6")]
public abstract class DefinitionAssetValueReference : DefinitionAssetValue
{
	public override Type   InspectorControlType => typeof(InspectorReferenceControl);
	public abstract string Name { get; }

	public          string Guid;

	public override void WriteJsonValue(StringWriter stringWriter)
	{
		//TODO
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValueReference))]
public interface IDefinitionAssetValueReferenceMigratable : IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueReference))]
public class DefinitionAssetValueReference_000 : DefinitionAssetValue_000, IDefinitionAssetValueReferenceMigratable
{
	public string Guid;
}
#endregion