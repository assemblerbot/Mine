using ImageMagick;

namespace Mine.Framework;

public sealed class ImageReference : Reference<Image>
{
	public ImageReference(string path) : base(path)
	{
	}

	public override Image? LoadValue()
	{
		try
		{
			byte[]? bytes = Engine.Resources.ReadResource(Path);
			if (bytes is null)
			{
				return null;
			}

			MagickImage image = new (bytes);
			
			//image.Width
			//image.GetPixels();
			return null;

		}
		catch (Exception e)
		{
			return null;
		}
	}
}