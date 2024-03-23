using Migration;
using Mine.Studio;

namespace Mine.Studio;

[Serializable, SerializedClassId("8589a74d-43fc-4c5d-8537-87681debc89e")]
public sealed class ImporterSceneMaterialSettings
{
	[ReadOnlyInInspector] public string Name;

	public ImporterSceneMaterialSettings(string name)
	{
		Name = name;
	}
}

#region Migration

[MigratableInterface(typeof(ImporterSceneMaterialSettings))]
public interface IImporterSceneMaterialSettingsMigratable;

[Serializable, LatestVersion(typeof(ImporterSceneMaterialSettings))]
public class ImporterSceneMaterialSettings_000 : IImporterSceneMaterialSettingsMigratable
{
	public string Name;
}
#endregion