using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("c62608e0-7dd6-4235-97e3-9462ad82726c")]
public abstract class DefinitionAssetValue
{
	public abstract Type InspectorControlType { get; }
	public abstract void WriteJsonValue(StringWriter stringWriter);
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValue))]
public interface IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValue))]
public class DefinitionAssetValue_000 : IDefinitionAssetValueMigratable
{
}
#endregion