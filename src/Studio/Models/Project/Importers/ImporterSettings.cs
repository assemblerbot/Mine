using Migration;
using RedHerring.Studio.Models.Project.FileSystem;

namespace RedHerring.Studio.Models.Project.Importers;

[Serializable, SerializedClassId("1fbf8638-5ae8-49cd-be7c-846bbadd6951")]
public abstract class ImporterSettings
{
	public abstract ProjectNodeType NodeType { get; }
}

#region Migration
[MigratableInterface(typeof(ImporterSettings))]
public interface IImporterSettingsMigratable
{
}
    
[Serializable, LatestVersion(typeof(ImporterSettings))]
public abstract class ImporterSettings_000 : IImporterSettingsMigratable
{
}
#endregion