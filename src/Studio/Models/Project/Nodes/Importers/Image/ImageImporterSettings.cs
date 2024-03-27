using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("663d4118-54fd-4b07-9664-93a4938a1fc3")]
public sealed class ImageImporterSettings : ImporterSettings
{
	public float NormalSmoothingAngle = 15f;
	public bool  CompensateFBXScale   = false;
}

#region Migration
[MigratableInterface(typeof(ImageImporterSettings))]
public interface IImageImporterSettingsMigratable : IImporterSettingsMigratable;

[Serializable, LatestVersion(typeof(ImageImporterSettings))]
public class ImageImporterSettings_000 : ImporterSettings_000, IImageImporterSettingsMigratable
{
	public float SmoothingAngle;
	public bool  CompensateFBXScale;
}
#endregion