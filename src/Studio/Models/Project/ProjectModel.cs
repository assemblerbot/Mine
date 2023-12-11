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
	
	private ProjectFolderNode? _assetsFolder;
	public  ProjectFolderNode? AssetsFolder => _assetsFolder;

	private ProjectFolderNode? _scriptsGameLibraryFolder;
	public  ProjectFolderNode? ScriptsGameLibraryFolder => _scriptsGameLibraryFolder; 

	private ProjectSettings? _projectSettings;
	public  ProjectSettings ProjectSettings => _projectSettings!;
	
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
		_thread.ClearQueue();
		
		SaveSettings();
		_assetsFolder = null;
		_eventAggregator.Trigger(new ClosedEvent());
	}
	
	public void Open(string projectPath)
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
			RecursiveAssetScan(assetsPath, "", assetsFolder);
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

		InitMeta();
		ImportAll();
		_eventAggregator.Trigger(new OpenedEvent());
	}

	private void RecursiveAssetScan(string path, string relativePath, ProjectFolderNode root)
	{
		// scan directories
		foreach (string directoryPath in Directory.EnumerateDirectories(path))
		{
			string            directory             = Path.GetFileName(directoryPath);
			string            relativeDirectoryPath = Path.Combine(relativePath, directory);
			ProjectFolderNode folderNode            = new(directory, directoryPath, relativeDirectoryPath, true, ProjectNodeType.AssetFolder);
			root.Children.Add(folderNode);
			
			RecursiveAssetScan(directoryPath, relativeDirectoryPath, folderNode);
		}

		// scan files except meta
		foreach (string filePath in Directory.EnumerateFiles(path))
		{
			string fileName = Path.GetFileName(filePath);
			if (fileName.EndsWith(".meta"))
			{
				continue;
			}

			string          relativeFilePath = Path.Combine(relativePath, fileName);
			ProjectAssetFileNode assetFileNode         = new(fileName, filePath, relativeFilePath);
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
			string fileName = Path.GetFileName(filePath);
			string               relativeFilePath = Path.Combine(relativePath, fileName);
			ProjectScriptFileNode assetFileNode    = new(fileName, filePath, relativeFilePath);
			root.Children.Add(assetFileNode);
		}
	}

	private void InitMeta()
	{
		_assetsFolder!.TraverseRecursive(
			node => _thread.Enqueue(new ProjectTask(cancellationToken => node.InitMeta(_migrationManager, cancellationToken))),
			TraverseFlags.Directories | TraverseFlags.Files,
			default
		);

		_scriptsGameLibraryFolder!.TraverseRecursive(
			node => _thread.Enqueue(new ProjectTask(cancellationToken => node.InitMeta(_migrationManager, cancellationToken))),
			TraverseFlags.Directories | TraverseFlags.Files,
			default
		);
	}

	#endregion
	
	#region Import
	public void ImportAll()
	{
		// TODO - delete everything from Resources?

		_assetsFolder!.TraverseRecursive(ImportProjectNode, TraverseFlags.Files, default);
	}

	private void ImportProjectNode(ProjectNode node)
	{
		_thread.Enqueue(
			new ProjectTask(
				cancellationToken =>
				{
					Importer importer = _importerRegistry.GetImporter(node.Extension);
					node.Meta.ImporterSettings ??= importer.CreateSettings();
					node.Type = node.Meta.ImporterSettings.NodeType;

					string resourcePath = Path.Combine(_projectSettings!.AbsoluteResourcesPath, node.RelativePath);

					using Stream stream = File.OpenRead(node.Path);
					ImporterResult result = importer.Import(stream, node.Meta.ImporterSettings, resourcePath, cancellationToken);

					if (result == ImporterResult.FinishedSettingsChanged)
					{
						node.UpdateMetaFile();
					}
				}
			)
		);
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
		File.WriteAllBytes(Path.Join(_projectSettings.GameFolderPath, _settingsFileName), json);
	}

	public void LoadSettings(string projectPath)
	{
		string path = Path.Join(projectPath, _settingsFileName);
		if(!File.Exists(path))
		{
			_projectSettings = new ProjectSettings
                              {
                                  GameFolderPath = projectPath
                              };
			return;
		}
		
		byte[] json = File.ReadAllBytes(path);
		ProjectSettings settings = MigrationSerializer.DeserializeAsync<ProjectSettings, IStudioSettingsMigratable>(_migrationManager.TypesHash, json, SerializedDataFormat.JSON, _migrationManager, false, Assembly).GetAwaiter().GetResult();
		settings.GameFolderPath = projectPath;
		
		_projectSettings = settings;
	}
	#endregion
}