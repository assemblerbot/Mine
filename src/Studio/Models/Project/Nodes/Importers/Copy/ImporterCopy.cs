using Mine.Framework;

namespace Mine.Studio;

[Importer]
public sealed class ImporterCopy : Importer<byte[]>
{
	public override string ReferenceType => nameof(AssetReference);
	
	public ImporterCopy(ProjectNode owner) : base(owner)
	{
	}

	public override void UpdateCache()
	{
	}

	public override byte[]? Load()
	{
		try
		{
			return File.ReadAllBytes(Owner.AbsolutePath);
		}
		catch (Exception e)
		{
			ConsoleViewModel.LogException(e.ToString());
		}

		return null;
	}

	public override void Save(byte[] data)
	{
		throw new InvalidOperationException();
	}

	public override void ClearCache()
	{
	}

	public override void Import(string resourcesRootPath, out string? relativeResourcePath)
	{
		byte[]? data = Load();
		
		if (data == null)
		{
			ConsoleViewModel.LogError($"Cannot import file {Owner.RelativePath}, file was not loaded!");
			relativeResourcePath = null;
			return;
		}

		string path = Path.Join(resourcesRootPath, Owner.RelativePath);
		
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path)!);
			File.WriteAllBytes(path, data);
		}
		catch (Exception e)
		{
			ConsoleViewModel.LogException(e.ToString());
		}

		relativeResourcePath = Owner.RelativePath;
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new ImporterCopySettings();
	}

	public override bool UpdateImportSettings(ImporterSettings settings)
	{
		return false;
	}
}