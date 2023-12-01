using System.Reflection;
using Migration;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.ViewModels.Console;

namespace RedHerring.Studio.Models.Project;

public sealed class ProjectModel
{
	private const string _assetsFolderName = "Assets";
	private const string _settingsFileName = "Project.json";

	public static Assembly Assembly => typeof(ProjectModel).Assembly; 

	private readonly MigrationManager _migrationManager;
	
	private ProjectFolderNode? _assetsFolder;
	public  ProjectFolderNode? AssetsFolder => _assetsFolder;

	private ProjectSettings? _projectSettings;
	public  ProjectSettings? ProjectSettings => _projectSettings;
	
	public ProjectModel(MigrationManager migrationManager)
	{
		_migrationManager = migrationManager;
	}
	
	#region Open/close
	public async Task CloseAsync()
	{
		await SaveSettingsAsync();
		_assetsFolder = null;
	}
	
	public async Task OpenAsync(string projectPath)
	{
		await LoadSettingsAsync(projectPath);
		
		string            assetsPath   = Path.Join(projectPath, _assetsFolderName);
		ProjectFolderNode assetsFolder = new ProjectRootNode(_assetsFolderName, assetsPath);

		if (!Directory.Exists(assetsPath))
		{
			// error
			ConsoleViewModel.Log($"Assets folder not found on path {assetsPath}", ConsoleItemType.Error);
			await assetsFolder.InitMetaRecursive(_migrationManager); // create meta for at least root
			return;
		}
		
		//Console.WriteLine($"Scan project path {projectPath}");

		try
		{
			RecursiveScan(assetsPath, "", assetsFolder);
		}
		catch (Exception e)
		{
			ConsoleViewModel.Log($"Exception: {e}", ConsoleItemType.Exception);
		}

		await assetsFolder.InitMetaRecursive(_migrationManager);
		_assetsFolder = assetsFolder;
	}

	private void RecursiveScan(string path, string relativePath, ProjectFolderNode root)
	{
		// scan directories
		foreach (string directoryPath in Directory.EnumerateDirectories(path))
		{
			string            directory             = Path.GetFileName(directoryPath);
			string            relativeDirectoryPath = Path.Combine(relativePath, directory);
			ProjectFolderNode folderNode            = new(directory, directoryPath, relativeDirectoryPath);
			root.Children.Add(folderNode);
			
			RecursiveScan(directoryPath, relativeDirectoryPath, folderNode);
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
			ProjectFileNode fileNode         = new(fileName, filePath, relativeFilePath);
			root.Children.Add(fileNode);
		}
	}
	#endregion
	
	#region Import
	
	#endregion
	
	#region Settings
	public async Task SaveSettingsAsync()
	{
		if (_projectSettings == null)
		{
			return;
		}

		byte[] json = await MigrationSerializer.SerializeAsync(_projectSettings, SerializedDataFormat.JSON, Assembly);
		await File.WriteAllBytesAsync(Path.Join(_projectSettings.GameFolderPath, _settingsFileName), json);
	}

	public async Task LoadSettingsAsync(string projectPath)
	{
		string path = Path.Join(projectPath, _settingsFileName);
		if(!File.Exists(path))
		{
			ConsoleViewModel.Log("Project settings not found, creating new", ConsoleItemType.Warning);
			_projectSettings = new ProjectSettings
                              {
                                  GameFolderPath = projectPath
                              };
			return;
		}
		
		byte[] json = await File.ReadAllBytesAsync(path);
		ProjectSettings settings = await MigrationSerializer.DeserializeAsync<ProjectSettings, IStudioSettingsMigratable>(_migrationManager.TypesHash, json, SerializedDataFormat.JSON, _migrationManager, false, Assembly);
		settings.GameFolderPath = projectPath;
		
		_projectSettings = settings;
	}
	#endregion
}