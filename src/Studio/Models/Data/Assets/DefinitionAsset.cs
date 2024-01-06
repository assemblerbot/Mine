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

	public static DefinitionAsset? CreateFromStream(Stream stream, MigrationManager migrationManager)
	{
		byte[] json = new byte[stream.Length];
		int    read = stream.Read(json, 0, json.Length);
		DefinitionAsset asset = MigrationSerializer.Deserialize<DefinitionAsset, IDefinitionAssetMigratable>(migrationManager.TypesHash, json, SerializedDataFormat.JSON, migrationManager);
		return asset;
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

	public void ImportToResources(string path)
	{
		StringWriter stringWriter = new ();

		stringWriter.Write('[');
		for(int rowIndex=0; rowIndex<Rows.Count; ++rowIndex)
		{
			DefinitionAssetRow row = Rows[rowIndex];

			stringWriter.Write('{');
			for (int fieldIndex = 0; fieldIndex < Template.Fields.Count; ++fieldIndex)
			{
				DefinitionTemplateField? field = Template.Fields[fieldIndex];
				if (field == null)
				{
					continue;
				}

				stringWriter.Write('"');
				stringWriter.Write(field.Name);
				stringWriter.Write('"');
				
				stringWriter.Write(':');

				row.Values[fieldIndex].WriteJsonValue(stringWriter);

				if (fieldIndex < Template.Fields.Count - 1)
				{
					stringWriter.Write(',');
				}
			}
			stringWriter.Write('}');
			
			if (rowIndex < Rows.Count - 1)
			{
				stringWriter.Write(',');
			}
		}
		stringWriter.Write(']');

		File.WriteAllText(path, stringWriter.ToString());
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