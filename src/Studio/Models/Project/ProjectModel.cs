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
	
	private const string _assetsFolderName             = "Assets";
	private const string _scriptsGameLibraryFolderName = "GameLibrary";
	private const string _settingsFileName             = "Project.json";
	
	public static Assembly Assembly => typeof(ProjectModel).Assembly; 

	private readonly MigrationManager           _migrationManager;
	private readonly StudioModelEventAggregator _eventAggregator;
	private readonly ImporterRegistry           _importerRegistry = new();

	public readonly object ProjectTreeLock = new(); // synchronization lock
	
	private ProjectFolderNode? _assetsFolder;
	public  ProjectFolderNode? AssetsFolder => _assetsFolder;

	private ProjectFolderNode? _scriptsGameLibraryFolder;
	public  ProjectFolderNode? ScriptsGameLibraryFolder => _scriptsGameLibraryFolder; 

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
			string            assetsPath   = Path.Join(projectPath, _assetsFolderName);
			ProjectFolderNode assetsFolder = new ProjectRootNode(_assetsFolderName, assetsPath, ProjectNodeType.AssetFolder);
			_assetsFolder = assetsFolder;

			string            scriptsGameLibraryPath   = Path.Join(projectPath, _scriptsGameLibraryFolderName);
			ProjectFolderNode scriptsGameLibraryFolder = new ProjectRootNode(_scriptsGameLibraryFolderName, scriptsGameLibraryPath, ProjectNodeType.ScriptFolder);
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

		ProjectFolderNode? currentNode = _assetsFolder;
		char[]             slash       = {'/', '\\'};
		string             pathToCheck = evt.Name;
		while (pathToCheck.Length > 0 && currentNode != null)
		{
			int index = pathToCheck.IndexOfAny(slash);
			if (index >= 0)
			{
				// folder
				string       folderName = pathToCheck.Substring(0, index);
				ProjectNode? childNode  = currentNode.FindChild(folderName);
				if (childNode == null)
				{
					// folder is not present - create new
					ProjectFolderNode newFolderNode = new ProjectFolderNode(
						folderName,
						Path.Join(currentNode.Path,         folderName),
						Path.Join(currentNode.RelativePath, folderName),
						true,
						ProjectNodeType.AssetFolder
					);

					// enqueue tasks for new node
					EnqueueProjectTaskFromWatcher(CreateInsertNodeTask(currentNode, newFolderNode));
					EnqueueProjectTaskFromWatcher(CreateInitMetaTask(newFolderNode));
					
					// next
					currentNode = newFolderNode;
				}
				else if (childNode is ProjectFolderNode childFolderNode)
				{
					// folder is present - next
					currentNode = childFolderNode;
				}
				else
				{
					// file system entity with that name is file, that is not allowed
					ConsoleViewModel.LogError($"File and folder with the same name on the same path is not allowed! {evt.FullPath}");
					return;
				}

				pathToCheck = pathToCheck.Substring(folderName.Length + 1);
				continue;
			}
			
			// no more subfolders, rest of path is file or folder itself
			if (Directory.Exists(evt.FullPath))
			{
				// it's a folder
				ProjectFolderNode assetFolderNode = new(pathToCheck, evt.FullPath, evt.Name, true, ProjectNodeType.AssetFolder);
				EnqueueProjectTaskFromWatcher(CreateInsertNodeTask(currentNode, assetFolderNode));
				EnqueueProjectTaskFromWatcher(CreateInitMetaTask(assetFolderNode));
				break;
			}
			
			// it's a file
			ProjectAssetFileNode assetFileNode = new(pathToCheck, evt.FullPath, evt.Name);
			EnqueueProjectTaskFromWatcher(CreateInsertNodeTask(currentNode, assetFileNode));
			EnqueueProjectTaskFromWatcher(CreateInitMetaTask(assetFileNode));
			EnqueueProjectTaskFromWatcher(CreateImportTask(assetFileNode));
			break;
		}
	}

	private void OnAssetDeleted(object sender, FileSystemEventArgs evt)
	{
		
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
	private ProjectTask CreateInsertNodeTask(ProjectFolderNode parent, ProjectNode child)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					parent.Children.Add(child);
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
					using Stream   stream = File.OpenRead(node.Path);
					ImporterResult result = importer.Import(stream, node.Meta.ImporterSettings, resourcePath, cancellationToken);

					if (result == ImporterResult.FinishedSettingsChanged)
					{
						node.UpdateMetaFile();
					}
				}
				catch (Exception e)
				{
					ConsoleViewModel.LogError($"While importing file {node.Path} an exception occured: {e}");
				}
			}
		);
	}
	#endregion
}