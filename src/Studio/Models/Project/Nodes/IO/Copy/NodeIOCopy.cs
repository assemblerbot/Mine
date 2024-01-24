using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;
using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

[NodeIO]
public sealed class NodeIOCopy : NodeIO
{
	private byte[]? _cache = null;

	public NodeIOCopy(ProjectNode owner) : base(owner)
	{
	}

	public override void Load()
	{
		if (_cache != null)
		{
			return;
		}

		try
		{
			_cache = File.ReadAllBytes(Owner.AbsolutePath);
		}
		catch (Exception e)
		{
			ConsoleViewModel.LogException(e.ToString());
		}
	}

	public override void Save()
	{
		throw new InvalidOperationException();
	}

	public override void ClearCache()
	{
		_cache = null;
	}

	public override void Import(string resourcePath)
	{
		Load();
		
		if (_cache == null)
		{
			ConsoleViewModel.LogError($"Cannot import file {Owner.RelativePath}, file was not loaded!");
			return;
		}

		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);
			File.WriteAllBytes(resourcePath, _cache);
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
}