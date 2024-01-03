using Migration;

namespace Mine.Studio;

[Serializable]
public abstract class DefinitionAssetValue
{
	public abstract Type InspectorControlType { get; }
}

#region Migration

[MigratableInterface(typeof(DefinitionAssetValue))]
public interface IDefinitionAssetValueMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValue))]
public class DefinitionAssetValue_000 : IDefinitionAssetValueMigratable
{
}
#endregion