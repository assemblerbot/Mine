using System.Reflection;
using EventAggregatorPlugin;
using Migration;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.ViewModels;
using RedHerring.Studio.Models.ViewModels.Console;
using RedHerring.Studio.TaskProcessing;

namespace RedHerring.Studio.Models;

// main studio model
public class StudioModel
{
	private const int _threadsCount = 4;
	
	public static    Assembly         Assembly => typeof(StudioModel).Assembly; 
	private readonly MigrationManager _migrationManager = new(Assembly);
	
	private readonly ProjectModel _project;
	public           ProjectModel Project => _project;

	private StudioSettings  _studioSettings = new();
	public  StudioSettings  StudioSettings => _studioSettings;

	private CommandHistory _commandHistory = new();
	public  CommandHistory CommandHistory => _commandHistory;
	
	// view models
	private readonly ConsoleViewModel _console = new();
	public           ConsoleViewModel Console => _console;
	
	private readonly SelectionViewModel _selection = new();
	public SelectionViewModel Selection => _selection;

	private readonly TaskProcessor _taskProcessor = new(_threadsCount);
	public           TaskProcessor TaskProcessor => _taskProcessor;
	
	// events
	private readonly StudioModelEventAggregator          _eventAggregator = new();
	public           IStudioModelEventAggregatorReadOnly EventAggregator => _eventAggregator;

	public StudioModel()
	{
		_project = new ProjectModel(_migrationManager, _eventAggregator);
	}

	public void Close()
	{
		_taskProcessor.Cancel();
		Project.CloseAsync().GetAwaiter().GetResult();
	}

	public async Task OpenProjectAsync(string path)
	{
		Selection.DeselectAll();
		Project.CloseAsync().GetAwaiter().GetResult();
		
		try
		{
			ConsoleViewModel.Log($"Opening project from {path}", ConsoleItemType.Info);
			await Project.OpenAsync(path);
			ConsoleViewModel.Log($"Project opened", ConsoleItemType.Success);
		}
		catch (Exception e)
		{
			ConsoleViewModel.Log($"Exception: {e}", ConsoleItemType.Exception);
		}
	}

	public void RunTests()
	{
		for(int i=0;i <20;++i)
		{
			_taskProcessor.EnqueueTask(new TestTask(i), 0);
		}
	}

	public void SaveStudioSettings()
	{
		byte[] json = MigrationSerializer.SerializeAsync(StudioSettings, SerializedDataFormat.JSON, Assembly).GetAwaiter().GetResult();
		Directory.CreateDirectory(Path.GetDirectoryName(StudioSettings.SettingsPath)!);
		File.WriteAllBytes(StudioSettings.SettingsPath, json);
	}

	public void LoadStudioSettings()
	{
		if(!File.Exists(StudioSettings.SettingsPath))
		{
			return;
		}
		
		byte[] json = File.ReadAllBytes(StudioSettings.SettingsPath);
		StudioSettings settings = MigrationSerializer.DeserializeAsync<StudioSettings, IStudioSettingsMigratable>(_migrationManager.TypesHash, json, SerializedDataFormat.JSON, _migrationManager, false, Assembly).GetAwaiter().GetResult();
		_studioSettings = settings;
	}
}