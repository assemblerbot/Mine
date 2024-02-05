using System.IO.Compression;

namespace Mine.Framework;

public sealed class Resources
{
	private const string DefaultResourcePackageExtension = ".zip";
	
	private readonly List<string>                           _resourcePackages   = new();
	private readonly Dictionary<string, ResourceDescriptor> _resourceDictionary = new();

	private AssetDatabase _assetDatabase = new();
	
	public void Init()
	{
		ScanResourcePackages();
		BuildDictionaryFromPackages();
		BuildDictionaryFromFiles();

		_assetDatabase = AssetDatabase.LoadOrCreate(Engine.ResourcesPath);

		//debug
		foreach(var pair in _resourceDictionary)
		{
			Console.WriteLine($"Resource: {pair.Key} in {pair.Value.SourceFilePath} as {pair.Value.ResourceSourceType}");
		}
	}

	#region Reading
	public string? GetPathByGuid(string guid)
	{
		return _assetDatabase[guid]?.Path;
	}

	public byte[]? ReadResourceByGuid(string guid)
	{
		string? path = _assetDatabase[guid]?.Path;
		if (path is null)
		{
			return null;
		}

		return ReadResource(path);
	}

	public byte[]? ReadResource(string path)
	{
		if (!_resourceDictionary.TryGetValue(path, out ResourceDescriptor descriptor))
		{
			return null; // resource not found
		}

		switch (descriptor.ResourceSourceType)
		{
			case ResourceSourceType.Zip:
				return ReadResourceFromPackage(path, descriptor);
			case ResourceSourceType.File:
				return ReadResourceFromFile(path, descriptor);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private byte[]? ReadResourceFromFile(string path, ResourceDescriptor descriptor)
	{
		return File.ReadAllBytes(descriptor.SourceFilePath);
	}

	private byte[]? ReadResourceFromPackage(string path, ResourceDescriptor descriptor)
	{
		using ZipArchive archive = ZipFile.OpenRead(descriptor.SourceFilePath);
		ZipArchiveEntry? entry = archive.GetEntry(path);
		if (entry == null)
		{
			//throw new Exception($"Resource {path} not found in package {descriptor.FilePath}");
			return null;
		}

		using Stream stream = entry.Open();
		byte[] buffer = new byte[entry.Length];
		int readLength = stream.Read(buffer, 0, buffer.Length);
		return readLength == entry.Length ? buffer : null;
	}
	#endregion
	
	#region Scanning
	private void ScanResourcePackages()
	{
		if (!Directory.Exists(Engine.ResourcesPath))
		{
			Console.WriteLine("Resources folder not found!");
			return;
		}

		foreach (string fileName in Directory.EnumerateFiles(Engine.ResourcesPath))
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
					
				_resourceDictionary[entry.FullName] = new ResourceDescriptor(ResourceSourceType.Zip, package); // later packages should override resources stored in previous
			}
		}
	}
	
	private void BuildDictionaryFromFiles()
	{
		if (!Directory.Exists(Engine.ResourcesPath))
		{
			return;
		}

		BuildDictionaryFromFilesInDirectoryRecursive("");
	}

	private void BuildDictionaryFromFilesInDirectoryRecursive(string relativePath)
	{
		int resourceDirectoryLength = Engine.ResourcesPath.Length + 1;
		
		string absolutePath = Path.Combine(Engine.ResourcesPath, relativePath);
		foreach (string absoluteFilePath in Directory.EnumerateFiles(absolutePath))
		{
			if (absoluteFilePath.EndsWith(DefaultResourcePackageExtension))
			{
				continue;
			}

			string relativeFilePath = absoluteFilePath.Substring(resourceDirectoryLength).Replace('\\', '/');
			_resourceDictionary[relativeFilePath] = new ResourceDescriptor(ResourceSourceType.File, absoluteFilePath);
		}

		foreach (string absoluteDirectoryPath in Directory.EnumerateDirectories(absolutePath))
		{
			if (absoluteDirectoryPath.StartsWith("."))
			{
				continue;
			}

			string relativeDirectoryPath = absoluteDirectoryPath.Substring(resourceDirectoryLength);
			BuildDictionaryFromFilesInDirectoryRecursive(relativeDirectoryPath);
		}
	}
	#endregion
}