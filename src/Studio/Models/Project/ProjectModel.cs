using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Cryptography;
using Migration;
using Mine.Framework;
using Mine.Studio;
using OdinSerializer;
using RedHerring.Studio.Models.Project.FileSystem;
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
	
	private const           string ASSETS_FOLDER_NAME  = "Assets";
	private const           string SCRIPTS_FOLDER_NAME = "GameLibrary";
	private const           string SETTINGS_FILE_NAME  = "Project.json";
	private static readonly char[] _slash              = {'/', '\\'};
	
	public static           Assembly      Assembly => typeof(ProjectModel).Assembly; 
	private static readonly HashAlgorithm _hashAlgorithm = SHA1.Create();

	// TODO - move to studio model
	private readonly StudioModelEventAggregator _eventAggregator;

	public readonly object ProjectTreeLock = new(); // synchronization lock
	
	private ProjectRootNode? _assetsFolder;
	public  ProjectRootNode? AssetsFolder => _assetsFolder;

	private ProjectRootNode? _scriptsFolder;
	public  ProjectRootNode? ScriptsFolder => _scriptsFolder;

	private ConcurrentDictionary<string, ProjectNode> _nodesIndex = new();

	private ProjectSettings? _projectSettings;
	public  ProjectSettings  ProjectSettings => _projectSettings!;

	private StudioAssetDatabase? _assetDatabase;

	public bool IsOpened               => !string.IsNullOrEmpty(_projectSettings?.ProjectFolderPath);
	public bool NeedsUpdateEngineFiles { get; private set; } = false;

	private          FileSystemWatcher?           _assetsWatcher;
	private          FileSystemWatcher?           _scriptsWatcher;
	private          bool                         _watchersActive      = true;
	private readonly ConcurrentQueue<ProjectTask> _waitingWatcherTasks = new();
	
	private readonly ProjectThread _thread = new ();
	public           int           TasksCount => _thread.TasksCount;
	
	public ProjectModel(StudioModelEventAggregator eventAggregator)
	{
		_eventAggregator = eventAggregator;
	}

	public void Cancel()
	{
		_thread.Cancel();
	}

	// TODO - this is bad, kind of project database is needed, indexation in this class will be just too messy
	public ProjectNode? FindNodeByGuid(string guid)
	{
		return FindNode(node => node.Meta?.Guid == guid, true, true);
	}
	
	// TODO - this is bad, kind of project database is needed, indexation in this class will be just too messy
	public ProjectNode? FindNode(Func<ProjectNode, bool> predicate, bool inAssets, bool inScripts)
	{
		ProjectNode?            resultNode              = null;
		CancellationTokenSource cancellationTokenSource = new();

		if (inAssets)
		{
			_assetsFolder!.TraverseRecursive(
				node =>
				{
					if (predicate(node))
					{
						resultNode = node;
						cancellationTokenSource.Cancel();
					}
				},
				TraverseFlags.Files | TraverseFlags.Directories,
				cancellationTokenSource.Token
			);

			if (resultNode != null)
			{
				return resultNode;
			}
		}

		if (inScripts)
		{
			_scriptsFolder!.TraverseRecursive(
				node =>
				{
					if (predicate(node))
					{
						resultNode = node;
						cancellationTokenSource.Cancel();
					}
				},
				TraverseFlags.Files | TraverseFlags.Directories,
				cancellationTokenSource.Token
			);
		}

		return resultNode;
	}
	
	#region Open/close
	public void Close()
	{
		DisposeWatchers();
		
		_thread.ClearQueue();
		
		SaveSettings();
		_assetsFolder  = null;
		_scriptsFolder = null;
		_nodesIndex.Clear();
		_assetDatabase = null;
		_eventAggregator.Trigger(new ClosedEvent());
	}
	
	public void Open(string projectPath)
	{
		lock (ProjectTreeLock)
		{
			// create roots
			string          assetsPath   = Path.Join(projectPath, ASSETS_FOLDER_NAME);
			ProjectRootNode assetsFolder = new ProjectRootNode(this, ASSETS_FOLDER_NAME, assetsPath, ProjectNodeType.AssetFolder);
			_assetsFolder = assetsFolder;

			string          scriptsGameLibraryPath   = Path.Join(projectPath, SCRIPTS_FOLDER_NAME);
			ProjectRootNode scriptsGameLibraryFolder = new ProjectRootNode(this, SCRIPTS_FOLDER_NAME, scriptsGameLibraryPath, ProjectNodeType.ScriptFolder);
			_scriptsFolder = scriptsGameLibraryFolder;

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
			
			// create asset database
			_assetDatabase = new StudioAssetDatabase();

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

		EnqueueProjectTask(CreateEngineFilesCheckTask(projectPath));
		
		InitMeta();

		//ImportAll();
		_eventAggregator.Trigger(new OpenedEvent());

		CreateWatchers();
	}
	#endregion
	
	#region Import and resources
	public void ClearResources()
	{
		EnqueueProjectTask(CreateClearResourcesTask(_projectSettings!.AbsoluteResourcesPath));

		lock (ProjectTreeLock)
		{
			_assetsFolder!.TraverseRecursive(
				node => EnqueueProjectTask(CreateResetMetaHashTask(_assetsFolder!, node.RelativePath)),
				TraverseFlags.Directories | TraverseFlags.Files,
				default
			);
		}
	}

	public void ImportAll()
	{
		lock (ProjectTreeLock)
		{
			_assetsFolder!.TraverseRecursive(
				node => EnqueueProjectTask(CreateImportFileTask(_assetsFolder!, node.RelativePath)),
				TraverseFlags.Files,
				default
			);

			_assetsFolder!.TraverseRecursive(
				node => EnqueueProjectTask(CreateImportFolderTask(_assetsFolder!, node.RelativePath)),
				TraverseFlags.Directories,
				default
			);

			EnqueueProjectTask(CreateSaveAssetDatabaseTask());
		}
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
		_assetsWatcher.Renamed               += OnAssetRenamed;
		_assetsWatcher.Changed               += OnAssetChanged;
		_assetsWatcher.Filter                =  "";
		_assetsWatcher.IncludeSubdirectories =  true;
		_assetsWatcher.EnableRaisingEvents   =  true;

		_scriptsWatcher                       =  new FileSystemWatcher(_projectSettings!.AbsoluteScriptsPath);
		_scriptsWatcher.NotifyFilter          =  NotifyFilters.DirectoryName | NotifyFilters.FileName;
		_scriptsWatcher.Created               += OnScriptCreated;
		_scriptsWatcher.Deleted               += OnScriptDeleted;
		_scriptsWatcher.Renamed               += OnScriptRenamed;
		_scriptsWatcher.Changed               += OnScriptChanged;
		_scriptsWatcher.Filter                =  "";
		_scriptsWatcher.IncludeSubdirectories =  true;
		_scriptsWatcher.EnableRaisingEvents   =  true;
	}

	private void OnAssetCreated(object sender, FileSystemEventArgs evt)
	{
		if (evt.Name == null)
		{
			return;
		}

		OnAssetCreated(evt.FullPath, evt.Name);
	}

	private void OnAssetDeleted(object sender, FileSystemEventArgs evt)
	{
		if (evt.Name == null)
		{
			return;
		}

		OnAssetDeleted(evt.FullPath, evt.Name);
	}

	private void OnAssetRenamed(object sender, RenamedEventArgs evt)
	{
		if (evt.Name == null || evt.OldName == null)
		{
			return;
		}

		OnAssetDeleted(evt.OldFullPath, evt.OldName);
		OnAssetCreated(evt.FullPath, evt.Name);
	}
 
	private void OnAssetChanged(object sender, FileSystemEventArgs evt)
	{
		if (evt.Name == null)
		{
			return;
		}

		OnAssetChanged(evt.FullPath, evt.Name);
	}
	
	private void OnScriptCreated(object sender, FileSystemEventArgs evt)
	{
		if (evt.Name == null)
		{
			return;
		}

		if (!Directory.Exists(evt.FullPath) && !evt.Name.EndsWith(".cs"))
		{
			return;
		}

		OnScriptCreated(evt.FullPath, evt.Name);
	}
	
	private void OnScriptDeleted(object sender, FileSystemEventArgs evt)
	{
		if (evt.Name == null)
		{
			return;
		}

		OnScriptDeleted(evt.FullPath, evt.Name);
	}

	private void OnScriptRenamed(object sender, RenamedEventArgs evt)
	{
		if (evt.Name == null || evt.OldName == null)
		{
			return;
		}

		if (!Directory.Exists(evt.FullPath) && !evt.Name.EndsWith(".cs"))
		{
			return;
		}
		
		OnScriptDeleted(evt.OldFullPath, evt.OldName);
		OnScriptCreated(evt.FullPath, evt.Name);
	}

	private void OnScriptChanged(object sender, FileSystemEventArgs evt)
	{
		if (evt.Name == null || !evt.Name.EndsWith(".cs"))
		{
			return;
		}

		OnScriptChanged(evt.FullPath, evt.Name);
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

		byte[] json = MigrationSerializer.Serialize(_projectSettings, SerializedDataFormat.JSON, Assembly);
		File.WriteAllBytes(Path.Join(_projectSettings.ProjectFolderPath, SETTINGS_FILE_NAME), json);
	}

	public void LoadSettings(string projectPath)
	{
		string path = Path.Join(projectPath, SETTINGS_FILE_NAME);
		if(!File.Exists(path))
		{
			_projectSettings = new ProjectSettings
			                   {
				                   ProjectFolderPath = projectPath
			                   };
			return;
		}
		
		byte[] json = File.ReadAllBytes(path);
		ProjectSettings settings = MigrationSerializer.Deserialize<ProjectSettings, IStudioSettingsMigratable>(StudioGlobals.MigrationManager.TypesHash, json, SerializedDataFormat.JSON, StudioGlobals.MigrationManager, false, Assembly);
		settings.ProjectFolderPath = projectPath;
		
		_projectSettings = settings;
	}
	#endregion
	
	#region Asset manipulation
	public void LoadAssetNode<TData>(ProjectAssetFileNode node, Action<TData?>? onComplete)
	{
		ProjectTask task = CreateLoadNodeTask(_assetsFolder!, node.RelativePath, onComplete);
		EnqueueProjectTask(task);
	}

	public void SaveAssetNode<TData>(ProjectAssetFileNode node, TData data, Action<bool>? onComplete)
	{
		ProjectTask task = CreateSaveNodeTask(_assetsFolder!, node.RelativePath, data, onComplete);
		EnqueueProjectTask(task);
	}

	private void RecursiveAssetScan(string path, string relativePath, ProjectFolderNode root, List<string> foundMetaFiles)
	{
		// scan directories
		foreach (string directoryPath in Directory.EnumerateDirectories(path))
		{
			string            directory             = Path.GetFileName(directoryPath);
			string            relativeDirectoryPath = Path.Combine(relativePath, directory);
			ProjectFolderNode folderNode            = new(this, directory, directoryPath, relativeDirectoryPath, true, ProjectNodeType.AssetFolder);
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
			ProjectAssetFileNode assetFileNode    = new(this, fileName, filePath, relativeFilePath);
			root.Children.Add(assetFileNode);
		}
	}
	
	private void InitMeta()
	{
		lock (ProjectTreeLock)
		{
			_assetsFolder!.TraverseRecursive(
				node => _thread.Enqueue(CreateInitNodeTask(_assetsFolder!, node.RelativePath)),
				TraverseFlags.Directories | TraverseFlags.Files,
				default
			);

			_scriptsFolder!.TraverseRecursive(
				node => _thread.Enqueue(CreateInitNodeTask(_scriptsFolder!, node.RelativePath)),
				TraverseFlags.Directories | TraverseFlags.Files,
				default
			);
		}
	}

	private void OnAssetCreated(string eventAbsolutePath, string eventRelativePath)
	{
		if (eventRelativePath.EndsWith(".meta"))
		{
			return;
		}

		string relativePath = "";
		string path         = eventRelativePath;
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
				EnqueueProjectTaskFromWatcher(CreateInitNodeTask(_assetsFolder!, relativePath));
			}
			else
			{
				if (Directory.Exists(eventAbsolutePath))
				{
					// created directory
					EnqueueProjectTaskFromWatcher(CreateNewFolderNodeTask(_assetsFolder!, relativePath, path, true, ProjectNodeType.AssetFolder));
					EnqueueProjectTaskFromWatcher(CreateInitNodeTask(_assetsFolder!, eventRelativePath));
				}
				else
				{
					// created file
					EnqueueProjectTaskFromWatcher(CreateNewAssetFileTask(_assetsFolder!, relativePath, path));
					EnqueueProjectTaskFromWatcher(CreateInitNodeTask(_assetsFolder!, eventRelativePath));
					//EnqueueProjectTaskFromWatcher(CreateImportTask(_assetsFolder!, eventRelativePath));
				}
				break;
			}
		}
	}

	private void OnAssetDeleted(string eventAbsolutePath, string eventRelativePath)
	{
		if (eventRelativePath.EndsWith(".meta"))
		{
			// deleted meta file .. create new
			string assetRelativeFilePath = eventRelativePath.Substring(0, eventRelativePath.Length - ".meta".Length);
			EnqueueProjectTaskFromWatcher(CreateInitNodeTask(_assetsFolder!, assetRelativeFilePath));
			return;
		}

		// deleted folder or file
		{
			int    index      = eventRelativePath.LastIndexOfAny(_slash);
			string parentPath = index == -1 ? "" : eventRelativePath.Substring(0, index);
			string nodeName   = eventRelativePath.Substring(index + 1);
			EnqueueProjectTaskFromWatcher(CreateDeleteNodeTask(_assetsFolder!, parentPath, nodeName));
		}
	}

	private void OnAssetChanged(string eventAbsolutePath, string eventRelativePath)
	{
		if (eventRelativePath.EndsWith(".meta"))
		{
			return;
		}

		EnqueueProjectTaskFromWatcher(CreateUpdateNodeTask(_assetsFolder!, eventRelativePath));
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
	
	#region Script manipulation
	public void LoadScriptNode<TData>(ProjectScriptFileNode node, Action<TData?>? onComplete)
	{
		ProjectTask task = CreateLoadNodeTask(_scriptsFolder!, node.RelativePath, onComplete);
		EnqueueProjectTask(task);
	}

	public void SaveScriptNode<TData>(ProjectScriptFileNode node, TData data, Action<bool>? onComplete)
	{
		ProjectTask task = CreateSaveNodeTask(_scriptsFolder!, node.RelativePath, data, onComplete);
		EnqueueProjectTask(task);
	}

	public ProjectScriptFileNode? FindScriptNodeByGuid(string guid)
	{
		ProjectNode? resultNode = null;

		_scriptsFolder!.TraverseRecursive(
			node =>
			{
				if (node.Meta?.Guid == guid)
				{
					resultNode = node;
				}
			},
			TraverseFlags.Files,
			new CancellationToken()
		);

		return resultNode as ProjectScriptFileNode;
	}
	
	private void RecursiveScriptScan(string path, string relativePath, ProjectFolderNode root)
	{
		// scan directories
		foreach (string directoryPath in Directory.EnumerateDirectories(path))
		{
			string            directory             = Path.GetFileName(directoryPath);
			string            relativeDirectoryPath = Path.Combine(relativePath, directory);
			ProjectFolderNode folderNode            = new(this, directory, directoryPath, relativeDirectoryPath, false, ProjectNodeType.ScriptFolder);
			root.Children.Add(folderNode);
			
			RecursiveScriptScan(directoryPath, relativeDirectoryPath, folderNode);
		}

		// scan all files
		foreach (string filePath in Directory.EnumerateFiles(path))
		{
			string                fileName         = Path.GetFileName(filePath);
			string                relativeFilePath = Path.Combine(relativePath, fileName);
			ProjectScriptFileNode assetFileNode    = new(this, fileName, filePath, relativeFilePath);
			root.Children.Add(assetFileNode);
		}
	}

	private void OnScriptCreated(string eventAbsolutePath, string eventRelativePath)
	{
		string relativePath = "";
		string path         = eventRelativePath;
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
				EnqueueProjectTaskFromWatcher(CreateNewFolderNodeTask(_scriptsFolder!, parentRelativePath, folderName, false, ProjectNodeType.ScriptFolder));
			}
			else
			{
				if (Directory.Exists(eventAbsolutePath))
				{
					// created directory
					EnqueueProjectTaskFromWatcher(CreateNewFolderNodeTask(_scriptsFolder!, relativePath, path, false, ProjectNodeType.ScriptFolder));
				}
				else
				{
					// created file
					EnqueueProjectTaskFromWatcher(CreateNewScriptFileTask(_scriptsFolder!, relativePath, path));
					EnqueueProjectTaskFromWatcher(CreateInitNodeTask(_scriptsFolder!, eventRelativePath));
				}
				break;
			}
		}
	}

	private void OnScriptDeleted(string eventAbsolutePath, string eventRelativePath)
	{
		{
			int    index      = eventRelativePath.LastIndexOfAny(_slash);
			string parentPath = index == -1 ? "" : eventRelativePath.Substring(0, index);
			string nodeName   = eventRelativePath.Substring(index + 1);
			EnqueueProjectTaskFromWatcher(CreateDeleteNodeTask(_scriptsFolder!, parentPath, nodeName));
		}
	}
	
	private void OnScriptChanged(string eventAbsolutePath, string eventRelativePath)
	{
		EnqueueProjectTaskFromWatcher(CreateUpdateNodeTask(_scriptsFolder!, eventRelativePath));
	}
	#endregion
	
	#region Engine files manipulation
	public void UpdateEngineFiles()
	{
		EnqueueProjectTask(CreateEngineFilesUpdateTask(_projectSettings!.ProjectFolderPath));
		EnqueueProjectTask(CreateEngineFilesCheckTask(_projectSettings!.ProjectFolderPath));
	}
	#endregion
	
	#region Tasks
	private void EnqueueProjectTask(ProjectTask task)
	{
		_thread.Enqueue(task);
	}
	
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
								this,
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
								this,
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

	private ProjectTask CreateNewScriptFileTask(ProjectRootNode root, string parentPath, string name)
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
							ProjectScriptFileNode newNode = new(
								this,
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
	
	private ProjectTask CreateSaveAssetDatabaseTask()
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					_assetDatabase!.Save(_projectSettings!);
				}
			}
		);
	}
	
	private ProjectTask CreateInitNodeTask(ProjectRootNode root, string path)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					ProjectNode? node = root.FindNode(path);
					if (node != null && node.Exists)
					{
						node.Init(cancellationToken);
					}
				}
			}
		);
	}

	private ProjectTask CreateResetMetaHashTask(ProjectRootNode root, string path)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					ProjectNode? node = root.FindNode(path);
					if (node != null && node.Exists)
					{
						node.ResetMetaHash();
					}
				}
			}
		);
	}

	private ProjectTask CreateImportFileTask(ProjectRootNode root, string path)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					// check node
					ProjectAssetFileNode? node = root.FindNode(path) as ProjectAssetFileNode;
					if (node == null || node.Meta == null)
					{
						return;
					}

					// check file
					if (!File.Exists(node.AbsolutePath))
					{
						return;
					}

					// calculate hash
					string? hash = null;
					try
					{
						using FileStream file = new(node.AbsolutePath, FileMode.Open);
						hash = Convert.ToBase64String(_hashAlgorithm.ComputeHash(file)); // how to cancel compute hash?
					}
					catch (Exception e)
					{
						return;
					}

					// check hash
					if (node.Meta.Hash == hash)
					{
						return;
					}

					try
					{
						NodeIO  io           = node.GetNodeIO<NodeIO>()!;
						string? resourcePath = io.Import(_projectSettings!.AbsoluteResourcesPath);
						if (resourcePath is not null)
						{
							_assetDatabase![node.Meta.Guid!] = new StudioAssetDatabaseItem(node.Meta.Guid!, node.Meta.Field, resourcePath, io.ReferenceType);
						}
					}
					catch (Exception e)
					{
						ConsoleViewModel.LogError($"While importing file {node.AbsolutePath} an exception occured: {e}");
						return;
					}

					// update hash
					node.Meta.SetHash(hash);
				}
			}
		);
	}

	private ProjectTask CreateImportFolderTask(ProjectRootNode root, string path)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					// check node
					ProjectFolderNode? node = root.FindNode(path) as ProjectFolderNode;
					if (node == null || node.Meta == null)
					{
						return;
					}

					// check folder
					if (!Directory.Exists(node.AbsolutePath))
					{
						return;
					}

					// just create folder
					try
					{
						string resourcePath = Path.Join(_projectSettings!.AbsoluteResourcesPath, node.RelativePath);
						Directory.CreateDirectory(resourcePath);
					}
					catch (Exception e)
					{
						ConsoleViewModel.LogException(e.ToString());
					}
					
					// add to database
					if (node != root)
					{
						_assetDatabase![node.Meta.Guid!] = new StudioAssetDatabaseItem(node.Meta.Guid!, node.Meta.Field, node.RelativePath, nameof(FolderReference));
					}
				}
			}
		);
	}

	private ProjectTask CreateDeleteNodeTask(ProjectRootNode root, string parentPath, string name)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					ProjectFolderNode? parent = root.FindNode(parentPath) as ProjectFolderNode;
					if (parent != null)
					{
						int index = parent.Children.FindIndex(child => child.Name == name);
						if (index == -1)
						{
							return;
						}

						ProjectNode child    = parent.Children[index];
						string      metaPath = child.AbsolutePath + ".meta";
						if (File.Exists(metaPath))
						{
							File.Delete(metaPath);
						}

						parent.Children.RemoveAt(index);
					}
				}
			}
		);
	}

	private ProjectTask CreateEngineFilesCheckTask(string projectRootPath)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				NeedsUpdateEngineFiles = TemplateUtility.NeedsUpdateFromTemplate(projectRootPath);
			}
		);
	}

	private ProjectTask CreateEngineFilesUpdateTask(string projectRootPath)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				TemplateUtility.UpdateLibrariesFromTemplate(projectRootPath);
			}
		);
	}

	private ProjectTask CreateClearResourcesTask(string absoluteResourcesPath)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				try
				{
					if (Directory.Exists(absoluteResourcesPath))
					{
						Directory.Delete(absoluteResourcesPath, true);
					}

					Directory.CreateDirectory(absoluteResourcesPath);
				}
				catch (Exception e)
				{
					ConsoleViewModel.LogException(e.ToString());
				}
			}
		);
	}

	private ProjectTask CreateLoadNodeTask<TData>(ProjectRootNode root, string path, Action<TData?>? onComplete)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					ProjectNode? node = root.FindNode(path);
					if (node != null && node.Exists)
					{
						NodeIO<TData>? io = node.GetNodeIO<NodeIO<TData>>();
						if (io is not null)
						{
							TData? data = io.Load();
							onComplete?.Invoke(data);
							return;
						}
					}

					onComplete?.Invoke(default);
				}
			}
		);
	}

	private ProjectTask CreateSaveNodeTask<TData>(ProjectRootNode root, string path, TData data, Action<bool>? onComplete)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					ProjectNode? node = root.FindNode(path);
					if (node != null && node.Exists)
					{
						NodeIO<TData>? io = node.GetNodeIO<NodeIO<TData>>();
						if (io is not null)
						{
							io.Save(data);
							onComplete?.Invoke(true);
							return;
						}
					}
					onComplete?.Invoke(false);
				}
			}
		);
	}

	private ProjectTask CreateUpdateNodeTask(ProjectRootNode root, string path)
	{
		return new ProjectTask(
			cancellationToken =>
			{
				lock (ProjectTreeLock)
				{
					ProjectNode? node = root.FindNode(path);
					if (node != null && node.Exists)
					{
						NodeIO? io = node.GetNodeIO<NodeIO>();
						if (io is not null)
						{
							io.UpdateCache();
						}
					}
				}
			}
		);
	}

	#endregion
}