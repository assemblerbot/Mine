/*
using Migration;
using SixLabors.ImageSharp.Formats.Png;

namespace Mine.Studio;

[Serializable, SerializedClassId("79e3e75f-a6c2-465b-b815-fd68fb46972a")]
public sealed class PngNodeIOSettings : NodeIOSettings
{
	public override ProjectNodeType NodeType => ProjectNodeType.AssetImage;

    public PngBitDepth BitDepth = PngBitDepth.Bit8;
    public PngColorType ColorType = PngColorType.RgbWithAlpha;
    public PngCompressionLevel Compression = PngCompressionLevel.BestSpeed;
    public bool UseGamma = false;
    public bool PreserveTransparentColors = true;
}

#region Migration
[MigratableInterface(typeof(PngNodeIOSettings))]
public interface IPngNodeIOSettingsMigratable : INodeIOSettingsMigratable;

[Serializable, LatestVersion(typeof(PngNodeIOSettings))]
public class PngNodeIOSettings_000 : NodeIOSettings_000, IPngNodeIOSettingsMigratable
{
    public PngBitDepth BitDepth;
    public PngColorType ColorType;
    public PngCompressionLevel Compression;
    public bool UseGamma;
    public bool PreserveTransparentColors;
}
#endregion
*/