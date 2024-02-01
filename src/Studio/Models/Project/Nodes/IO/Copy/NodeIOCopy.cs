using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;
using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

[NodeIO]
public sealed class NodeIOCopy : NodeIO<byte[]>
{
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

	public override void Import(string resourcePath)
	{
		byte[]? data = Load();
		
		if (data == null)
		{
			ConsoleViewModel.LogError($"Cannot import file {Owner.RelativePath}, file was not loaded!");
			return;
		}

		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);
			File.WriteAllBytes(resourcePath, data);
		}
		catch (Exception e)
		{
			ConsoleViewModel.LogException(e.ToString());
		}

		ClearCache();
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