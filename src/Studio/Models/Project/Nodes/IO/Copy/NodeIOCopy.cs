using Mine.Framework;

namespace Mine.Studio;

[NodeIO]
public sealed class NodeIOCopy : NodeIO<byte[]>
{
	public override string ReferenceType => nameof(AssetReference);
	
	public NodeIOCopy(ProjectNode owner) : base(owner)
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

	public override string? Import(string resourcesRootPath)
	{
		byte[]? data = Load();
		
		if (data == null)
		{
			ConsoleViewModel.LogError($"Cannot import file {Owner.RelativePath}, file was not loaded!");
			return null;
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

		return Owner.RelativePath;
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOCopySettings();
	}

	public override bool UpdateImportSettings(NodeIOSettings settings)
	{
		return false;
	}
}