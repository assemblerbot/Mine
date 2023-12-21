using System.Collections.Concurrent;
using System.Reflection;
using Migration;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Importers;
using RedHerring.Studio.Models.ViewModels.Console;

namespace RedHerring.Studio.Models.Project;

public sealed class ProjectModel
{
	#region Events
	public struct OpenedEvent : IStudioModelEvent
	{
	}

	public struct ClosedEvent : IStudioModelEvent
	{
	}
	#endregion
	
	private const  string _assetsFolderName             = "Assets";
	private const  string _scriptsGameLibraryFolderName = "GameLibrary";
	private const  string _settingsFileName             = "Project.json";
	private static char[] _slash                        = {'/', '\\'};
	
	public static Assembly Assembly => typeof(ProjectModel).Assembly; 

	private readonly MigrationManager           _migrationManager;
	private readonly StudioModelEventAggregator _eventAggregator;
	private readonly ImporterRegistry           _importerRegistry = new();

	public readonly object ProjectTreeLock = new(); // synchronization lock
	
	private ProjectRootNode? _assetsFolder;
	public  ProjectRootNode? AssetsFolder => _assetsFolder;

	private ProjectRootNode? _scriptsGameLibraryFolder;
	public  ProjectRootNode? ScriptsGameLibraryFolder => _scriptsGameLibraryFolder; 

	private ProjectSettings? _projectSettings;
	public  ProjectSettings  ProjectSettings => _projectSettings!;

	private FileSystemWatcher?           _assetsWatcher;
	private FileSystemWatcher?           _scriptsWatcher;
	private bool                         _watchersActive      = true;
	private ConcurrentQueue<ProjectTask> _waitingWatcherTasks = new();
	
	private readonly ProjectThread _thread = new ();
	
	public ProjectModel(MigrationManager migrationManager, StudioModelEventAggregator eventAggregator)
	{
		_migrationManager = migrationManager;
		_eventAggregator  = eventAggregator;
	}

	public void Cancel()
	{
		_thread.Cancel();
	}

	#region Open/close
	public void Close()
	{
		DisposeWatchers();
		
		_thread.ClearQueue();
		
		SaveSettings();
		_assetsFolder = null;
		_eventAggregator.Trigger(new ClosedEvent());
	}
	
	public void Open(string projectPath)
	{
		lock (ProjectTreeLock)
		{
			// create roots
			string          assetsPath   = Path.Join(projectPath, _assetsFolderName);
			ProjectRootNode assetsFolder = new ProjectRootNode(_assetsFolderName, assetsPath, ProjectNodeType.AssetFolder);
			_assetsFolder = assetsFolder;

			string          scriptsGameLibraryPath   = Path.Join(projectPath, _scriptsGameLibraryFolderName);
			ProjectRootNode scriptsGameLibraryFolder = new ProjectRootNode(_scriptsGameLibraryFolderName, scriptsGameLibraryPath, ProjectNodeType.ScriptFolder);
			_scriptsGameLibraryFolder = scriptsGameLibraryFolder;

			// check
			if (!Directory.Exists(assetsPath))
			{
				// error
				ConsoleViewModel.LogError($"Assets folder not found on path {assetsPath}");
				InitMeta();
				_eventAggregator.Trigger(new OpenedEvent());
				return;
			}

			// load / create settings
			LoadSettings(projectPath);

			// read assets
			try
			{
				List<string> foundMetaFiles = new();
				RecursiveAssetScan(assetsPath, "", assetsFolder, foundMetaFiles);
				MetaCleanup(foundMetaFiles);
			}
			catch (Exception e)
			{
				ConsoleViewModel.LogException($"Exception: {e}");
			}

			// read scripts
			try
			{
				RecursiveScriptScan(scriptsGameLibraryPath, "", scriptsGameLibraryFolder);
			}
			catch (Exception e)
			{
				ConsoleViewModel.LogException($"Exception: {e}");
			}
		}

		InitMeta();

		ImportAll();
		_eventAggregator.Trigger(new OpenedEvent());

		CreateWatchers();
	}

	private void RecursiveAssetScan(string path, string relativePath, ProjectFolderNode root, List<string> foundMetaFiles)
	{
		// scan directories
		foreach (string directoryPath in Directory.EnumerateDirectories(path))
		{
			string            directory             = Path.GetFileName(directoryPath);
			string            relativeDirectoryPath = Path.Combine(relativePath, directory);
			ProjectFolderNode folderNode            = new(directory, directoryPath, relativeDirectoryPath, true, ProjectNodeType.AssetFolder);
			root.Children.Add(folderNode);
			
			RecursiveAssetScan(directoryPath, relativeDirectoryPath, folderNode, foundMetaFiles);
		}

		// scan files except meta
		foreach (string filePath in Directory.EnumerateFiles(path))
		{
			string fileName = Path.GetFileName(filePath);
			if (fileName.EndsWith(".meta"))
			{
				foundMetaFiles.Add(filePath);
				continue;
			}

			string               relativeFilePath = Path.Combine(relativePath, fileName);
			ProjectAssetFileNode assetFileNode    = new(fileName, filePath, relativeFilePath);
			root.Children.Add(assetFileNode);
		}
	}

	private void RecursiveScriptScan(string path, string relativePath, ProjectFolderNode root)
	{
		// scan directories
		foreach (string directoryPath in Directory.EnumerateDirectories(path))
		{
			string            directory             = Path.GetFileName(directoryPath);
			string            relativeDirectoryPath = Path.Combine(relativePath, directory);
			ProjectFolderNode folderNode            = new(directory, directoryPath, relativeDirectoryPath, false, ProjectNodeType.ScriptFolder);
			root.Children.Add(folderNode);
			
			RecursiveScriptScan(directoryPath, relativeDirectoryPath, folderNode);
		}

		// scan all files
		foreach (string filePath in Directory.EnumerateFiles(path))
		{
			string                fileName         = Path.GetFileName(filePath);
			string                relativeFilePath = Path.Combine(relativePath, fileName);
			ProjectScriptFileNode assetFileNode    = new(fileName, filePath, relativeFilePath);
			root.Children.Add(assetFileNode);
		}
	}

	private void InitMeta()
	{
		lock (ProjectTreeLock)
		{
			_assetsFolder!.TraverseRecursive(
				node => _thread.Enqueue(CreateInitMetaTask(node)),
				TraverseFlags.Directories | TraverseFlags.Files,
				default
			);

			_scriptsGameLibraryFolder!.TraverseRecursive(
				node => _thread.Enqueue(CreateInitMetaTask(node)),
				TraverseFlags.Directories | TraverseFlags.Files,
				default
			);
		}
	}

	private void MetaCleanup(List<string> metaFiles)
	{
		foreach (string metaFile in metaFiles)
		{
			string path = metaFile.Substring(0, metaFile.Length - ".meta".Length);
			if (Directory.Exists(path) || File.Exists(path))
			{
				continue;
			}

			ConsoleViewModel.LogWarning($"Removing unused meta file: {metaFile}");
			File.Delete(metaFile);
		}
	}

	#endregion
	
	#region Import
	public void ImportAll()
	{
		lock (ProjectTreeLock)
		{
			_assetsFolder!.TraverseRecursive(ImportProjectNode, TraverseFlags.Files, default);
		}
	}

	private void ImportProjectNode(ProjectNode node)
	{
		_thread.Enqueue(CreateImportTask(node));
	}

	#endregion
	
	#region Folder watchers
	public void PauseWatchers()
	{
		_watchersActive = false;
	}

	public void ResumeWatchers()
	{
		while (!_waitingWatcherTasks.IsEmpty)
		{
			if (_waitingWatcherTasks.TryDequeue(out ProjectTask? task))
			{
				_thread.Enqueue(task);
			}
		}

		_watchersActive = true;
	}

	private void CreateWatchers()
	{
		_assetsWatcher                       =  new FileSystemWatcher(_projectSettings!.AbsoluteAssetsPath);
		_assetsWatcher.NotifyFilter          =  NotifyFilters.DirectoryName | NotifyFilters.FileName;
		_assetsWatcher.Created               += OnAssetCreated;
		_assetsWatcher.Deleted               += OnAssetDeleted;
		_assetsWatcher.Renamed               += OnAssetChanged;
		_assetsWatcher.Filter                =  "";
		_assetsWatcher.IncludeSubdirectories =  true;
		_assetsWatcher.EnableRaisingEvents   =  true;

		_scriptsWatcher                       =  new FileSystemWatcher(_projectSettings!.AbsoluteScriptsPath);
		_scriptsWatcher.NotifyFilter          =  NotifyFilters.DirectoryName | NotifyFilters.FileName;
		_scriptsWatcher.Created               += OnScriptChanged;
		_scriptsWatcher.Deleted               += OnScriptChanged;
		_scriptsWatcher.Renamed               += OnScriptChanged;
		_scriptsWatcher.Filter                =  "*.cs";
		_scriptsWatcher.IncludeSubdirectories =  true;
		_scriptsWatcher.EnableRaisingEvents   =  true;
	}

	private void OnAssetCreated(object sender, FileSystemEventArgs evt)
	{
		if (evt.Name == null || evt.Name.EndsWith(".meta"))
		{
			return;
		}

		string relativePath = "";
		string path = evt.Name;
		while (path.Length > 0)
		{
			int index = path.IndexOfAny(_slash);
			if (index >= 0)
			{
				string folderName = path.Substring(0, index);
				path = path.Substring(index + 1);

				string parentRelativePath = relativePath;
				relativePath = Path.Join(relativePath, folderName);

				// folder
				EnqueueProjectTaskFromWatcher(CreateNewFolderNodeTask(_assetsFolder!, parentRelativePath, folderName, true, ProjectNodeType.AssetFolder));
				EnqueueProjectTaskFromWatcher(CreateInitMetaTask(_assetsFolder!, relativePath));
			}
			else
			{
				if (Directory.Exists(evt.FullPath))
				{
					// created directory
					EnqueueProjectTaskFromWatcher(CreateNewFolderNodeTask(_assetsFolder!, relativePath, path, true, ProjectNodeType.AssetFolder));
					EnqueueProjectTaskFromWatcher(CreateInitMetaTask(_assetsFolder!, evt.Name));
				}
				else
				{
					// created file
					EnqueueProjectTaskFromWatcher(CreateNewAssetFileTask(_assetsFolder!, relativePath, path));
					EnqueueProjectTaskFromWatcher(CreateInitMetaTask(_assetsFolder!, evt.Name));
					EnqueueProjectTaskFromWatcher(CreateImportTask(_assetsFolder!, evt.Name));
				}
				break;
			}
		}
	}

	private void OnAssetDeleted(object sender, FileSystemEventArgs evt)
	{
		if (evt.Name == null)
		{
			return;
		}

		if (evt.Name.EndsWith(".meta"))
		{
			// deleted meta file .. create new
			string       assetFileName = evt.Name.Substring(0, evt.Name.Length - ".meta".Length);
			ProjectNode? assetFileNode = _assetsFolder!.FindNode(assetFileName);
			if (assetFileNode != null)
			{
				EnqueueProjectTaskFromWatcher(CreateInitMetaTask(assetFileNode));
				return;
			}
		}

		// deleted folder or file
		ProjectNode? assetNode = _assetsFolder!.FindNode(evt.Name);
		if (assetNode != null)
		{
			int    index      = evt.Name.LastIndexOfAny(_slash);
			string parentPath = evt.Name.Substring(0, index);
			string nodeName   = evt.Name.Substring(index + 1);
			if (_assetsFolder!.FindNode(parentPath) is ProjectFolderNode parentNode)
			{
				EnqueueProjectTaskFromWatcher(CreateDeleteNodeTask(parentNode, nodeName));
			}
		}
	}

	private void OnAssetChanged(object sender, FileSystemEventArgs evt)
	{
		ConsoleViewModel.LogInfo($"Asset changed. Name:{evt.Name} Change:{evt.ChangeType} Path:{evt.FullPath}");
	}

	private void OnScriptChanged(object sender, FileSystemEventArgs evt)
	{
		ConsoleViewModel.LogInfo($"Source changed. Name:{evt.Name} Change:{evt.ChangeType} Path:{evt.FullPath}");
	}

	private void DisposeWatchers()
	{
		_assetsWatcher?.Dispose();
		_assetsWatcher = null;
		
		_scriptsWatcher?.Dispose();
		_scriptsWatcher = null;
	}

	private void EnqueueProjectTaskFromWatcher(ProjectTask task)
	{
		if (_watchersActive)
		{
			_thread.Enqueue(task);
			return;
		}

		_waitingWatcherTasks.Enqueue(task);
	}
	#endregion
	
	#region Settings
	public void SaveSettings()
	{
		if (_projectSettings == null)
		{
			return;
		}

		byte[] json = MigrationSerializer.SerializeAsync(_projectSettings, SerializedDataFormat.JSON, Assembly).GetAwaiter().GetResult();
		File.WriteAllBytes(Path.Join(_projectSettings.ProjectFolderPath, _settingsFileName), json);
	}

	public void LoadSettings(string projectPath)
	{
		string path = Path.Join(projectPath, _settingsFileName);
		if(!File.Exists(path))
		{
			_projectSettings = new ProjectSettings
			                   {
				                   ProjectFolderPath = projectPath
			                   };
			return;
		}
		
		byte[] json = File.ReadAllBytes(path);
		ProjectSettings settings = MigrationSerializer.DeserializeAsync<ProjectSettings, IStudioSettingsMigratable>(_migrationManager.TypesHash, json, SerializedDataFormat.JSON, _migrationManager, false, Assembly).GetAwaiter().GetResult();
		settings.ProjectFolderPath = projectPath;
		
		_projectSettings = settings;
	}
	#endregion
	
	#region Tasks
	private ProjectTask CreateNewFolderNodeTask(ProjectRootNode root, string parentPath, string name, bool hasMetaFile, ProjectNodeType type)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					if (root.FindNode(parentPath) is ProjectFolderNode parentNode)
					{
						if (parentNode.FindChild(name) == null)
						{
							ProjectFolderNode newNode = new(
								name,
								Path.Join(parentNode.AbsolutePath, name),
								Path.Join(parentNode.RelativePath, name),
								hasMetaFile,
								type
							);

							parentNode.Children.Add(newNode);
						}
					}
				}
			}
		);
	}

	private ProjectTask CreateNewAssetFileTask(ProjectRootNode root, string parentPath, string name)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					if (root.FindNode(parentPath) is ProjectFolderNode parentNode)
					{
						if (parentNode.FindChild(name) == null)
						{
							ProjectAssetFileNode newNode = new(
								name,
								Path.Join(parentNode.AbsolutePath, name),
								Path.Join(parentNode.RelativePath, name)
							);

							parentNode.Children.Add(newNode);
						}
					}
				}
			}
		);
	}

	private ProjectTask CreateInitMetaTask(ProjectRootNode root, string path)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					ProjectNode? node = root.FindNode(path);
					node?.InitMeta(_migrationManager, cancellationToken);
				}
			}
		);
	}

	private ProjectTask CreateImportTask(ProjectRootNode root, string path)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				ProjectAssetFileNode? node = root.FindNode(path) as ProjectAssetFileNode;
				if (node == null)
				{
					return;
				}

				Importer importer = _importerRegistry.GetImporter(node.Extension);
				node.Meta!.ImporterSettings ??= importer.CreateSettings();
				node.SetNodeType(node.Meta.ImporterSettings.NodeType);

				string resourcePath = Path.Combine(_projectSettings!.AbsoluteResourcesPath, node.RelativePath);

				try
				{
					using Stream   stream = File.OpenRead(node.AbsolutePath);
					ImporterResult result = importer.Import(stream, node.Meta.ImporterSettings, resourcePath, cancellationToken);

					if (result == ImporterResult.FinishedSettingsChanged)
					{
						node.UpdateMetaFile();
					}
				}
				catch (Exception e)
				{
					ConsoleViewModel.LogError($"While importing file {node.AbsolutePath} an exception occured: {e}");
				}
			}
		);
	}
	
	
	private ProjectTask CreateInsertNodeTask(ProjectFolderNode parent, ProjectNode child)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					if (parent.FindChild(child.Name) == null)
					{
						parent.Children.Add(child);
					}
				}
			}
		);
	}

	private ProjectTask CreateDeleteNodeTask(ProjectFolderNode parent, string childName)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					int index = parent.Children.FindIndex(child => child.Name == childName);
					if (index == -1)
					{
						return;
					}

					ProjectNode child = parent.Children[index];
					File.Delete(child.AbsolutePath + ".meta");
					parent.Children.RemoveAt(index);
				}
			}
		);
	}

	private ProjectTask CreateInitMetaTask(ProjectNode node)
	{
		return new ProjectTask(cancellationToken => node.InitMeta(_migrationManager, cancellationToken));
	}

	private ProjectTask CreateImportTask(ProjectNode node)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				Importer importer = _importerRegistry.GetImporter(node.Extension);
				node.Meta!.ImporterSettings ??= importer.CreateSettings();
				node.SetNodeType(node.Meta.ImporterSettings.NodeType);

				string resourcePath = Path.Combine(_projectSettings!.AbsoluteResourcesPath, node.RelativePath);

				try
				{
					using Stream   stream = File.OpenRead(node.AbsolutePath);
					ImporterResult result = importer.Import(stream, node.Meta.ImporterSettings, resourcePath, cancellationToken);

					if (result == ImporterResult.FinishedSettingsChanged)
					{
						node.UpdateMetaFile();
					}
				}
				catch (Exception e)
				{
					ConsoleViewModel.LogError($"While importing file {node.AbsolutePath} an exception occured: {e}");
				}
			}
		);
	}
	#endregion
}