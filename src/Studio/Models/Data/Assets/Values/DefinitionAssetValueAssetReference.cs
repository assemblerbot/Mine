using Migration;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("4018303e-635a-4a9d-9aa0-368586747f10")]
public sealed class DefinitionAssetValueAssetReference : DefinitionAssetValue
{
	public override Type   InspectorControlType => typeof(InspectorStringControl); // TODO
	public          string Guid;

	public override void WriteJsonValue(StringWriter stringWriter)
	{
		//TODO
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValueAssetReference))]
public interface IDefinitionAssetValueAssetReferenceMigratable : IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueAssetReference))]
public class DefinitionAssetValueAssetReference_000 : DefinitionAssetValue_000, IDefinitionAssetValueAssetReferenceMigratable
{
	public string Guid;
}
#endregion