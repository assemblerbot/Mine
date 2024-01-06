using System.Text.Json;
using ImGuiNET;
using Mine.Framework;
using Mine.ImGuiPlugin;
using NativeFileDialogSharp;
using RedHerring.Studio.Models;
using RedHerring.Studio.Tools;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

public sealed class StudioComponent : Component, IUpdatable
{
	public int GetUpdateOrder() => 0;

	private readonly StudioModel _studioModel = new();

	private readonly ToolManager _toolManager = new();
	
	#region User Interface
	private readonly DockSpace               _dockSpace        = new();
	private readonly Menu                    _menu             = new(MenuStyle.MainMenu);
	private readonly StatusBar               _statusBar        = new();
	private          ObjectDialog            _projectSettings  = null!;
	private          ObjectDialog            _studioSettings   = null!;
	private          NewProjectDialog        _newProjectDialog = null!;
	private readonly MessageBox              _messageBox       = new();
	private readonly StatusBarMessageHandler _statusBarMessageHandler;
	#endregion

	public StudioComponent()
	{
		_statusBarMessageHandler = new StatusBarMessageHandler(_statusBar, _studioModel);
	}

	public override void AfterAddedToScene()
	{
		Engine.Scene.RegisterUpdatable(this);
		Engine.Instance.OnFocusChanged += OnFocusChanged;
		
		InitImGui();
		InitMenu();

		_toolManager.Init(_studioModel);

		_projectSettings  = new ObjectDialog("Project settings", _studioModel.CommandHistory, _studioModel.Project.ProjectSettings);
		_studioSettings   = new ObjectDialog("Studio settings",  _studioModel.CommandHistory, _studioModel.StudioSettings);
		_newProjectDialog = new NewProjectDialog(_studioModel);

		LoadSettings();
		
		Engine.Scene.Add(new GameObject("Test Object").AddComponent<TestRenderComponent>().GameObject);

		// List<int> test   = new() {2, 4, 6, 8, 10, 12, 13};
		// int       index0 = test.FindInsertionIndexBinary(x => x.CompareTo(0));
		// int       index1 = test.FindInsertionIndexBinary(x => x.CompareTo(15));
		// int       index2 = test.FindInsertionIndexBinary(x => x.CompareTo(5));
		// int       index3 = test.FindInsertionIndexBinary(x => x.CompareTo(4));
		// int       index4 = test.FindInsertionIndexBinary(x => x.CompareTo(2));
		// int       index5 = test.FindInsertionIndexBinary(x => x.CompareTo(12));
		//
		// int d=0;

		//Engine.Window.
	}

	public override void BeforeRemovedFromScene()
	{
		SaveSettings();
		
		_studioModel.Close();
		Engine.Scene.UnregisterUpdatable(this);
		Engine.Instance.OnFocusChanged -= OnFocusChanged;
	}

	public override void Dispose()
	{
		
	}

	public void Update(double timeDelta)
	{
		_dockSpace.Update();
		_menu.Update();
		_menu.InvokeClickActions();
		_statusBarMessageHandler.Update();
		_statusBar.Update();
		_projectSettings.Update();
		_studioSettings.Update();
		_messageBox.Update();
		_newProjectDialog.Update();

		_toolManager.Update();

		//ImGui.ShowDemoWindow();
	}
	
	private void InitImGui()
	{
		Engine.Scene.Add(new GameObject("ImGui").AddComponent<ImGuiComponent>().GameObject);
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
	}
	
	#region Menu
	private void InitMenu()
	{
		_menu.AddItem("File/New project..",  OnNewProjectClicked);
		_menu.AddItem("File/Open project..", OnOpenProjectClicked);
		_menu.AddItem("File/Exit",           OnExitClicked);

		_menu.AddItem("Edit/Undo",               _studioModel.CommandHistory.Undo);
		_menu.AddItem("Edit/Redo",               _studioModel.CommandHistory.Redo);
		_menu.AddItem("Edit/Project settings..", OnEditProjectSettingsClicked);
		_menu.AddItem("Edit/Studio settings..",  OnEditStudioSettingsClicked);

		_menu.AddItem("View/Project",     OnViewProjectClicked);
		_menu.AddItem("View/Console",     OnViewConsoleClicked);
		_menu.AddItem("View/Inspector",   OnViewInspectorClicked);
		_menu.AddItem("View/Plugins",     OnViewPluginsClicked);
		_menu.AddItem("View/Definitions", OnViewDefinitionsClicked);

		_menu.AddItem("Project/Update engine files", OnProjectUpdateEngineFilesClicked, () => _studioModel.Project.IsOpened);
		
		_menu.AddItem("Debug/Modal window",        () => ImGui.OpenPopup("MessageBox"));
		_menu.AddItem("Debug/Task processor test", OnDebugTaskProcessorTestClicked);
		_menu.AddItem("Debug/Serialization test",  OnDebugSerializationTestClicked);
		_menu.AddItem("Debug/Importer test",       OnDebugImporterTestClicked);
	}

	private void OnNewProjectClicked()
	{
		_newProjectDialog.Open();
	}

	private void OnOpenProjectClicked()
	{
		DialogResult result = Dialog.FolderPicker();
		if(!result.IsOk)
		{
			return;
		}
		
		_studioModel.OpenProject(result.Path);
	}

	private void OnExitClicked()
	{
		Engine.Exit();
	}

	private void OnFocusChanged(bool hasFocus)
	{
		if (hasFocus)
		{
			_studioModel.Project.ResumeWatchers();
		}
		else
		{
			_studioModel.Project.PauseWatchers();
		}
	}

	private void OnEditProjectSettingsClicked()
	{
		_projectSettings.Open();
	}

	private void OnEditStudioSettingsClicked()
	{
		_studioSettings.Open();
	}
	
	private void OnViewProjectClicked()
	{
		_toolManager.Activate(ToolProjectView.ToolName);
	}

	private void OnViewConsoleClicked()
	{
		_toolManager.Activate(ToolConsole.ToolName);
	}

	private void OnViewInspectorClicked()
	{
		_toolManager.Activate(ToolInspector.ToolName);
	}

	private void OnViewPluginsClicked()
	{
		_toolManager.Activate(ToolPlugins.ToolName);
	}
	
	private void OnViewDefinitionsClicked()
	{
		_toolManager.Activate(ToolDefinitions.ToolName);
	}

	private void OnProjectUpdateEngineFilesClicked()
	{
		_studioModel.Project.UpdateEngineFiles();
	}

	private void OnDebugTaskProcessorTestClicked()
	{
		// for(int i=0;i <20;++i)
		// {
		// 	_studioModel.TaskProcessor.EnqueueTask(new TestTask(i), 0);
		// }
	}

	private void OnDebugSerializationTestClicked()
	{
//		SerializationTests.Test();
		List<DefinitionTemplateTest> templates = new() {new DefinitionTemplateTest(), new DefinitionTemplateTest(), new DefinitionTemplateTest()};

		string json = JsonSerializer.Serialize(templates);
		File.WriteAllText("D:\\Tmp\\serialization_test.json", json);
	}

	private void OnDebugImporterTestClicked()
	{
//		ImporterTests.Test();
	}
	#endregion

	#region Settings
	private void SaveSettings()
	{
		_studioModel.StudioSettings.StoreToolWindows(Tool.UniqueToolIdGeneratorState, _toolManager.ExportActiveTools());
		
		_studioModel.StudioSettings.UiLayout = ImGui.SaveIniSettingsToMemory();
		_studioModel.SaveStudioSettings();
	}

	private void LoadSettings()
	{
		_studioModel.LoadStudioSettings();

		Tool.SetUniqueIdGenerator(_studioModel.StudioSettings.ToolUniqueIdGeneratorState);
		_toolManager.ImportActiveTools(_studioModel.StudioSettings.ActiveToolWindows);
		
		if (_studioModel.StudioSettings.UiLayout != null)
		{
			ImGui.LoadIniSettingsFromMemory(_studioModel.StudioSettings.UiLayout);
		}

		_studioModel.StudioSettings.ApplyTheme();
	}
	#endregion
}

	
public sealed class DefinitionTemplateTest
{
	//--- data begin ---
	public int    IntField    {get; private set;}
	public float  FloatField  {get; private set;}
	public string StringField {get; private set;}
	public bool   MyField     {get; private set;}
	//--- data end ---
}

/*
 
 [
    {
        "IntField": 0,
        "FloatField": 0,
        "StringField": null,
        "MyField": false
    },
    {
        "IntField": 0,
        "FloatField": 0,
        "StringField": null,
        "MyField": false
    },
    {
        "IntField": 0,
        "FloatField": 0,
        "StringField": null,
        "MyField": false
    }
]
 
 */