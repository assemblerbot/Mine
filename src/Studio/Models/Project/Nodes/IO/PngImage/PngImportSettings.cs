/*
using Migration;
using SixLabors.ImageSharp.Formats.Png;

namespace RedHerring.Studio.Models.Project.Imports;

[Serializable, SerializedClassId("79e3e75f-a6c2-465b-b815-fd68fb46972a")]
public sealed class PngImportSettings : ImportSettings
{
	public override ProjectNodeType NodeType => ProjectNodeType.AssetImage;

    public PngBitDepth BitDepth = PngBitDepth.Bit8;
    public PngColorType ColorType = PngColorType.RgbWithAlpha;
    public PngCompressionLevel Compression = PngCompressionLevel.BestSpeed;
    public bool UseGamma = false;
    public bool PreserveTransparentColors = true;
}

#region Migration
[MigratableInterface(typeof(PngImportSettings))]
public interface IPngImportSettingsMigratable : IImportSettingsMigratable;

[Serializable, LatestVersion(typeof(PngImportSettings))]
public class PngImportSettings_000 : ImportSettings_000, IPngImportSettingsMigratable
{
    public PngBitDepth BitDepth;
    public PngColorType ColorType;
    public PngCompressionLevel Compression;
    public bool UseGamma;
    public bool PreserveTransparentColors;
}
#endregion
*/