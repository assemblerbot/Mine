using ImageMagick;
using OdinSerializer;

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

			if (Path.EndsWith(".image"))
			{
				return SerializationUtility.DeserializeValue<Image>(bytes, DataFormat.Binary);
			}

			MagickImage image = new (bytes);
			
			return null;

		}
		catch (Exception e)
		{
			return null;
		}
	}
}