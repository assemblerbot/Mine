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

	public static DefinitionAsset? CreateFromFile(string absolutePath, MigrationManager migrationManager)
	{
		byte[] json = File.ReadAllBytes(absolutePath);
		DefinitionAsset asset = MigrationSerializer.Deserialize<DefinitionAsset, IDefinitionAssetMigratable>(migrationManager.TypesHash, json, SerializedDataFormat.JSON, migrationManager);
		return asset;
	}

	public void WriteToFile(string absolutePath)
	{
		byte[] json = MigrationSerializer.Serialize(this, SerializedDataFormat.JSON);
		File.WriteAllBytes(absolutePath, json);
	}

	public void ImportToResources(string absolutePath)
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

		File.WriteAllText(absolutePath, stringWriter.ToString());
	}

	public bool UpdateTemplate(DefinitionTemplate newTemplate)
	{
		bool changeDetected = false;
		
		List<DefinitionAssetRow> newRows         = Rows.Select(_ => new DefinitionAssetRow()).ToList();

		for (int newFieldIndex = 0; newFieldIndex < newTemplate.Fields.Count; ++newFieldIndex)
		{
			DefinitionTemplateField? newField = newTemplate.Fields[newFieldIndex];

			// uninitialized field, basically an error, but we have to keep the data
			if (newField == null)
			{
				changeDetected = true;
				for (int i = 0; i < Rows.Count; ++i)
				{
					newRows[i].Values.Add(new DefinitionAssetValueInt()); // TODO - null?
				}
				continue;
			}

			// find field in old template
			int oldFieldIndex = ((List<DefinitionTemplateField?>)_template.Fields).FindIndex( // nasty hack, but IReadOnlyList doesn't support basic search!
				field => field is not null && field.Name == newField.Name && field.Type == newField.Type && field.GenericParameter == newField.GenericParameter
			);

			if (oldFieldIndex != -1)
			{
				changeDetected |= newFieldIndex != oldFieldIndex;
				for (int i = 0; i < Rows.Count; ++i)
				{
					newRows[i].Values.Add(Rows[i].Values[oldFieldIndex]);
				}
				continue;
			}

			changeDetected = true;

			// try to match the name and convert type
			oldFieldIndex = ((List<DefinitionTemplateField?>)_template.Fields).FindIndex(
				field => field is not null && field.Name == newField.Name
			);

			if (oldFieldIndex != -1)
			{
				for (int i = 0; i < Rows.Count; ++i)
				{
					DefinitionAssetValue value = newField.Type.ToDefinitionAssetValue(newField.GenericParameter);
					newRows[i].Values.Add(value);
					value.ImportFrom(Rows[i].Values[oldFieldIndex]);
				}
				continue;
			}
			
			// if fields count doesn't match, use default values, probably new field was created
			if (_template.Fields.Count != newTemplate.Fields.Count)
			{
				for (int i = 0; i < Rows.Count; ++i)
				{
					DefinitionAssetValue value = newField.Type.ToDefinitionAssetValue(newField.GenericParameter);
					newRows[i].Values.Add(value);
				}
				continue;
			}
			
			// count is the same, try to find field by type and use it (field name must be also be different than anything else in new fields, to avoid duplication)
			oldFieldIndex = ((List<DefinitionTemplateField?>)_template.Fields).FindIndex(
				field => field is not null && field.Type == newField.Type && field.GenericParameter == newField.GenericParameter && newTemplate.Fields.All(f => f?.Name != field.Name)
			);

			if (oldFieldIndex != -1)
			{
				for (int i = 0; i < Rows.Count; ++i)
				{
					newRows[i].Values.Add(Rows[i].Values[oldFieldIndex]);
				}
				continue;
			}
			
			// fallback - nothing was found, create new value
			for (int i = 0; i < Rows.Count; ++i)
			{
				DefinitionAssetValue value = newField.Type.ToDefinitionAssetValue(newField.GenericParameter);
				newRows[i].Values.Add(value);
			}
		}
		
		// update template and rows
		_template = newTemplate;
		Rows      = newRows;

		return changeDetected;
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