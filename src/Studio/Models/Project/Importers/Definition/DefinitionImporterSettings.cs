using Migration;
using RedHerring.Studio.Models.Project.FileSystem;

namespace RedHerring.Studio.Models.Project.Importers;

[Serializable, SerializedClassId("7d81811d-0a7a-4e75-86b8-c99d389950b3")]
public sealed class DefinitionImporterSettings : ImporterSettings
{
	public override ProjectNodeType NodeType => ProjectNodeType.AssetDefinitionData;
}

#region Migration
[MigratableInterface(typeof(DefinitionImporterSettings))]
public interface IDefinitionImporterSettingsMigratable : IImporterSettingsMigratable;

[Serializable, LatestVersion(typeof(DefinitionImporterSettings))]
public class DefinitionImporterSettings_000 : ImporterSettings_000, IDefinitionImporterSettingsMigratable
{
}
#endregion