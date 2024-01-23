using Migration;
using RedHerring.Studio.Models.Project.Imports;
using RedHerring.Studio.Models.Project.Imports;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models.Project.FileSystem;

[Serializable, SerializedClassId("metadata-class-id")] // TODO - replace by GUID
public class Metadata
{
	[ReadOnlyInInspector] public string? Guid = null;
	[ReadOnlyInInspector] public string? Hash = null;

	public ImportSettings? ImportSettings = null;

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
	
	[MigrateField] public List<IImportSettingsMigratable>? ImportSettings;
	
	public void Migrate(Metadata_000 prev)
	{
		Guid = prev.Guid;
		Hash = null; // to force reimport
		
		ImportSettings = new List<IImportSettingsMigratable>();
	}
}

[Serializable, LatestVersion(typeof(Metadata))]
public class Metadata_002 : IMetadataMigratable
{
	public string? Guid;
	public string? Hash;
	
	[MigrateField] public IImportSettingsMigratable? ImportSettings;
	
	public void Migrate(Metadata_001 prev)
	{
		Guid = prev.Guid;
		Hash = null; // to force reimport
		
		ImportSettings = null;
	}
}
#endregion