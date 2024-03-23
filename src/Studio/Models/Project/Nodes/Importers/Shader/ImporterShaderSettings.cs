using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("733fc598-ff87-41c5-8f11-997f25d715a5")]
public sealed class ImporterShaderSettings : ImporterSettings
{
	public string            EntryPoint  = "main";
	public ImporterShaderStage ShaderStage = ImporterShaderStage.vertex;
}

#region Migration
[MigratableInterface(typeof(ImporterShaderSettings))]
public interface IImporterShaderSettingsMigratable : IImporterSettingsMigratable;

[Serializable, LatestVersion(typeof(ImporterShaderSettings))]
public class ImporterShaderSettings_000 : ImporterSettings_000, IImporterShaderSettingsMigratable
{
	public string            EntryPoint;
	public ImporterShaderStage ShaderStage;
}
#endregion