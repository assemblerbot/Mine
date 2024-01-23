using Migration;

namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("7d81811d-0a7a-4e75-86b8-c99d389950b3")]
public sealed class DefinitionImportSettings : ImportSettings
{
}

#region Migration
[MigratableInterface(typeof(DefinitionImportSettings))]
public interface IDefinitionImportSettingsMigratable : IImportSettingsMigratable;

[Serializable, LatestVersion(typeof(DefinitionImportSettings))]
public class DefinitionImportSettings_000 : ImportSettings_000, IDefinitionImportSettingsMigratable
{
}
#endregion