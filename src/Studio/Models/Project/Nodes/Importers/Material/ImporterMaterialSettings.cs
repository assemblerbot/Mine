using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("1423f1c3-fb3c-4b40-b1bb-363909e700bc")]
public sealed class ImporterMaterialSettings : ImporterSettings
{
}

#region Migration

[MigratableInterface(typeof(ImporterMaterialSettings))]
public interface IImporterMaterialSettingsMigratable;

[Serializable, LatestVersion(typeof(ImporterMaterialSettings))]
public class ImporterMaterialSettings_000 : IImporterMaterialSettingsMigratable
{
}
#endregion