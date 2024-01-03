using Migration;
using OdinSerializer;

namespace Mine.Studio;

[Serializable, SerializedClassId("d38c1f35-e309-41c4-8a99-6f5defad421a")]
public sealed class DefinitionAsset
{
	[OdinSerialize] private DefinitionTemplate _template;
	public                  DefinitionTemplate Template => _template;
	
	[OdinSerialize] public List<DefinitionAssetRow> Rows = new();

	public DefinitionAsset(DefinitionTemplate template)
	{
		_template = template;
	}

	public static DefinitionAsset? CreateFromFile(string path, MigrationManager migrationManager)
	{
		byte[] json = File.ReadAllBytes(path);
		DefinitionAsset asset = MigrationSerializer.Deserialize<DefinitionAsset, IDefinitionAssetMigratable>(migrationManager.TypesHash, json, SerializedDataFormat.JSON, migrationManager);
		return asset;
	}

	public void WriteToFile(string path)
	{
		byte[] json = MigrationSerializer.Serialize(this, SerializedDataFormat.JSON);
		File.WriteAllBytes(path, json);
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionAsset))]
public interface IDefinitionAssetMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionAsset))]
public class DefinitionAsset_000 : IDefinitionAssetMigratable
{
	public                IDefinitionTemplateMigratable       _template;
	[MigrateField] public List<IDefinitionAssetRowMigratable> _rows;
}
#endregion