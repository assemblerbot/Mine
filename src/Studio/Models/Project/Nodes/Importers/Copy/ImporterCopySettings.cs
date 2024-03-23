using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("d6112cef-1810-416d-b26b-9e9b75529b64")]
public sealed class ImporterCopySettings : ImporterSettings
{
}

#region Migration
[MigratableInterface(typeof(ImporterCopySettings))]
public interface IImporterCopySettingsMigratable : IImporterSettingsMigratable;

[Serializable, LatestVersion(typeof(ImporterCopySettings))]
public class ImporterCopySettings_000 : ImporterSettings_000, IImporterCopySettingsMigratable
{
}
#endregion