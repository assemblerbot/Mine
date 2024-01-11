using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("293da6d5-96f3-4057-9905-b20cffe6316a")]
public sealed class DefinitionAssetValueFolderReference : DefinitionAssetValueReference
{
	public override string Name => "Folder";
}

#region Migration
[MigratableInterface(typeof(DefinitionAssetValueFolderReference))]
public interface IDefinitionAssetValueFolderReferenceMigratable : IDefinitionAssetValueReferenceMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAssetValueFolderReference))]
public class DefinitionAssetValueFolderReference_000 : DefinitionAssetValueReference_000, IDefinitionAssetValueFolderReferenceMigratable
{
}
#endregion