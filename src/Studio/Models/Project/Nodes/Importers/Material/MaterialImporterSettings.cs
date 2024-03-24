using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("1423f1c3-fb3c-4b40-b1bb-363909e700bc")]
public sealed class MaterialImporterSettings : ImporterSettings
{
}

#region Migration

[MigratableInterface(typeof(MaterialImporterSettings))]
public interface IMaterialImporterSettingsMigratable;

[Serializable, LatestVersion(typeof(MaterialImporterSettings))]
public class MaterialImporterSettings_000 : IMaterialImporterSettingsMigratable
{
}
#endregion