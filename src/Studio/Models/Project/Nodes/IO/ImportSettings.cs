using Migration;
namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("1fbf8638-5ae8-49cd-be7c-846bbadd6951")]
public abstract class ImportSettings
{
}

#region Migration
[MigratableInterface(typeof(ImportSettings))]
public interface IImportSettingsMigratable
{
}
    
[Serializable, LatestVersion(typeof(ImportSettings))]
public abstract class ImportSettings_000 : IImportSettingsMigratable
{
}
#endregion