using ImageMagick;
using Veldrid;

namespace Mine.Framework;

[Serializable]
public sealed class Image
{
	public readonly int         Width;
	public readonly int         Height;
	public readonly PixelFormat Format;
	public readonly byte[]      Data;
	// TODO - mipmaps, cube maps

	public static Image CreateFromMagicImage(MagickImage magickImage)
	{
		magickImage.Format = MagickFormat.Rgba;
		magickImage.Depth  = 8;
	}
}