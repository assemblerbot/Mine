using Migration;
using OdinSerializer;

namespace Mine.Studio;

[Serializable, SerializedClassId("c9938d38-ceea-43f6-b167-ed7938b3fd58")]
public sealed class DefinitionAssetRow
{
	[OdinSerialize] private List<DefinitionAssetValue> _values = new();
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetRow))]
public interface IDefinitionAssetRowMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetRow))]
public class DefinitionAssetRow_000 : IDefinitionAssetRowMigratable
{
	[MigrateField] public List<IDefinitionAssetValueMigratable> _values = new();
}
#endregion