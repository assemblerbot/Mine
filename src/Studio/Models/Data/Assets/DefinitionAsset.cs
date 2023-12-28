using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("definition-asset")]
public sealed class DefinitionAsset
{
	private List<DefinitionAssetColumn> _columns = new();
	private List<DefinitionAssetRow>    _rows    = new();
}

#region Migration
[MigratableInterface(typeof(DefinitionAsset))]
public interface IDefinitionAssetMigratable
{
}
    
[Serializable, LatestVersion(typeof(DefinitionAsset))]
public class DefinitionAsset_000 : IDefinitionAssetMigratable
{
}
#endregion