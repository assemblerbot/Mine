using System.IO.Compression;

namespace GameToolkit.Framework;

public sealed class ResourceManager
{
	private const string DefaultResourcePackageExtension = ".zip";
	
	private readonly List<string>                           _resourcePackages   = new();
	private readonly Dictionary<string, ResourceDescriptor> _resourceDictionary = new();
	
	public void Init()
	{
		ScanResourcePackages();
		BuildDictionaryFromPackages();
		BuildDictionaryFromFiles();

		// debug
		// foreach(var pair in _resourceDictionary)
		// {
		// 	Console.WriteLine($"{pair.Key} in {pair.Value}");
		// }
	}

	private void ScanResourcePackages()
	{
		foreach (string fileName in Directory.EnumerateFiles(Engine.ResourceDirectory))
		{
			if (!fileName.EndsWith(DefaultResourcePackageExtension))
			{
				continue;
			}

			_resourcePackages.Add(fileName);
		}
		_resourcePackages.Sort();
	}

	private void BuildDictionaryFromPackages()
	{
		foreach(string package in _resourcePackages)
		{
			using ZipArchive archive = ZipFile.OpenRead(package);
			
			foreach (ZipArchiveEntry entry in archive.Entries)
			{
				if (entry.FullName.EndsWith("/"))
				{
					continue;
				}
					
				_resourceDictionary[entry.FullName] = new ResourceDescriptor(ResourceType.Zip, package); // later packages should override resources stored in previous
			}
		}
	}
	
	private void BuildDictionaryFromFiles()
	{
		BuildDictionaryFromFilesInDirectoryRecursive("");
	}

	private void BuildDictionaryFromFilesInDirectoryRecursive(string relativePath)
	{
		string directoryPath = Path.Combine(Engine.ResourceDirectory, relativePath);
		foreach (string fileName in Directory.EnumerateFiles(directoryPath))
		{
			if (!fileName.EndsWith(DefaultResourcePackageExtension))
			{
				continue;
			}
			
			_resourceDictionary[fileName] = new ResourceDescriptor(ResourceType.File, Path.Combine(relativePath, fileName));
		}

		foreach (string directory in Directory.EnumerateDirectories(directoryPath))
		{
			if (directory.StartsWith("."))
			{
				continue;
			}

			BuildDictionaryFromFilesInDirectoryRecursive(Path.Combine(relativePath, directory));
		}
	}
}