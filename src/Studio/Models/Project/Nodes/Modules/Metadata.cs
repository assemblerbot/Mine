using Migration;
using Mine.Studio;

namespace Mine.Studio;

[Serializable, SerializedClassId("0714ba10-3363-4221-ae09-53ad37dc2628")]
public class Metadata
{
	[ReadOnlyInInspector] public string? Guid  = null;
	[ReadOnlyInInspector] public string? Hash  = null;
	public                       string? ReferenceField = null;

	public ImporterSettings? ImporterSettings = null;

	public void UpdateGuid()
	{
		if (string.IsNullOrEmpty(Guid))
		{
			Guid = System.Guid.NewGuid().ToString();
		}
	}
	
	public void SetHash(string? hash)
	{
		Hash = hash;
	}
}

#region Migration
[MigratableInterface(typeof(Metadata))]
public interface IMetadataMigratable
{
}
    
[Serializable, ObsoleteVersion(typeof(Metadata))]
public class Metadata_000 : IMetadataMigratable
{
	public string? Guid;
	public string? Hash;
}

[Serializable, ObsoleteVersion(typeof(Metadata))]
public class Metadata_001 : IMetadataMigratable
{
	public string? Guid;
	public string? Hash;
	
	[MigrateField] public List<IImporterSettingsMigratable>? ImporterSettings;
	
	public void Migrate(Metadata_000 prev)
	{
		Guid  = prev.Guid;
		Hash  = null; // to force reimport
		
		ImporterSettings = new List<IImporterSettingsMigratable>();
	}
}

[Serializable, LatestVersion(typeof(Metadata))]
public class Metadata_002 : IMetadataMigratable
{
	public string? Guid;
	public string? Hash;
	public string? ReferenceField;
	
	[MigrateField] public IImporterSettingsMigratable? ImporterSettings;
	
	public void Migrate(Metadata_001 prev)
	{
		Guid  = prev.Guid;
		Hash  = null; // to force reimport
		ReferenceField = null;
		
		ImporterSettings = null;
	}
}
#endregion