using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("ce302b13-d585-42d1-99bc-73b988e5807f")]
public sealed class DefinitionAssetValueInt : DefinitionAssetValue
{
	public int Value;
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