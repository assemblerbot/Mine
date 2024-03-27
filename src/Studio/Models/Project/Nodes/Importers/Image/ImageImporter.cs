using ImageMagick;
using ImageMagick.Formats;
using Mine.Framework;

namespace Mine.Studio;

[Importer(ProjectNodeKind.AssetImage)]
public sealed class ImageImporter : Importer<MagickImage>
{
	public override string ReferenceType => nameof(AssetReference); // TODO - Image reference

	public ImageImporter(ProjectNode owner) : base(owner)
	{
	}
    
	public override void UpdateCache()
	{
	}

	public override void ClearCache()
	{
	}

	public override void Import(string resourcesRootPath, out string? relativeResourcePath)
	{
		ImageImporterSettings? settings = Owner.Meta?.ImporterSettings as ImageImporterSettings;
		if (settings is null)
		{
			ConsoleViewModel.LogError($"Cannot import '{Owner.RelativePath}'. Settings are missing or invalid!");
			relativeResourcePath = null;
			return;
		}

		try
		{
			using MagickImage image = new (Owner.AbsolutePath);
			if (image is null)
			{
				ConsoleViewModel.LogException($"Image '{Owner.RelativePath}' couldn't be imported.");
				relativeResourcePath = null;
				return;
			}

			// TODO - convert
			image.Format          = MagickFormat.Dds;
			var pixels = image.GetPixels();
			//pixels.
			//var compression = DdsCompression.Dxt5;

			relativeResourcePath = $"{Owner.RelativePath}.dds";
			string absolutePath = Path.Join(resourcesRootPath, relativeResourcePath);
			image.Write(absolutePath);
		}
		catch (Exception e)
		{
			ConsoleViewModel.LogException($"Image '{Owner.RelativePath}' import failed: {e}");
			relativeResourcePath = null;
		}
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ImageImporterSettings();
	}

	public override bool UpdateImportSettings(ImporterSettings settings)
	{
		return false;
	}

	public override MagickImage? Load()
	{
		throw new NotImplementedException();
	}

	public override void Save(MagickImage data)
	{
		throw new InvalidOperationException();
	}
}