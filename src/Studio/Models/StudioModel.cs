using System.Reflection;
using EventAggregatorPlugin;
using Migration;
using Mine.Studio;
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

	private static StudioModel _instance;
	public static  StudioModel Instance => _instance;
	
	private readonly ProjectModel _project;
	public           ProjectModel Project => _project;

	private StudioSettings  _studioSettings = new();
	public  StudioSettings  StudioSettings => _studioSettings;

	private CommandHistory _commandHistory = new();
	public  CommandHistory CommandHistory => _commandHistory;
	
	// view models
	private readonly ConsoleViewModel _console = new();
	public           ConsoleViewModel Console => _console;
	
	private readonly SelectionViewModel _selection;
	public SelectionViewModel Selection => _selection;

	private readonly TaskProcessor _taskProcessor = new(_threadsCount);
	public           TaskProcessor TaskProcessor => _taskProcessor;
	
	// events
	private readonly StudioModelEventAggregator          _eventAggregator = new();
	public           IStudioModelEventAggregatorReadOnly EventAggregator => _eventAggregator;

	public StudioModel()
	{
		_instance  = this;
		_project   = new ProjectModel(_eventAggregator);
		_selection = new(_eventAggregator);
	}

	public void Close()
	{
		_taskProcessor.Cancel();
		Project.Close();
	}

	public void OpenProject(string path)
	{
		Selection.DeselectAll();
		Project.Close();
		
		try
		{
			ConsoleViewModel.Log($"Opening project from {path}", ConsoleItemType.Info);
			Project.Open(path);
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
		byte[] json = MigrationSerializer.SerializeAsync(StudioSettings, SerializedDataFormat.JSON, StudioGlobals.Assembly).GetAwaiter().GetResult();
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
		StudioSettings settings = MigrationSerializer.DeserializeAsync<StudioSettings, IStudioSettingsMigratable>(StudioGlobals.MigrationManager.TypesHash, json, SerializedDataFormat.JSON, StudioGlobals.MigrationManager, false, StudioGlobals.Assembly).GetAwaiter().GetResult();
		_studioSettings = settings;
	}
}