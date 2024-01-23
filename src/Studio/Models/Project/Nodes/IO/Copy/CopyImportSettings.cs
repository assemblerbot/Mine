using Migration;
using RedHerring.Studio.Models.Project.FileSystem;

namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("d6112cef-1810-416d-b26b-9e9b75529b64")]
public sealed class CopyImportSettings : ImportSettings
{
}

#region Migration
[MigratableInterface(typeof(CopyImportSettings))]
public interface ICopyImportSettingsMigratable : IImportSettingsMigratable;

[Serializable, LatestVersion(typeof(CopyImportSettings))]
public class CopyImportSettings_000 : ImportSettings_000, ICopyImportSettingsMigratable
{
}
#endregion