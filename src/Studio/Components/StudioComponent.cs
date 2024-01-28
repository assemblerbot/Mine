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
	private          bool                    _showImGuiDemoWindow = false;
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

		if (_showImGuiDemoWindow)
		{
			ImGui.ShowDemoWindow();
		}
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
		_menu.AddItem("Project/Clear Resources",     OnProjectClearResourcesClicked,    () => _studioModel.Project.IsOpened);
		_menu.AddItem("Project/Reimport all",        OnProjectReimportAllClicked,       () => _studioModel.Project.IsOpened);
		_menu.AddItem("Project/Import changed",      OnProjectImportChangedClicked,       () => _studioModel.Project.IsOpened);
		
		_menu.AddItem("Debug/Modal window",        () => ImGui.OpenPopup("MessageBox"));
		_menu.AddItem("Debug/Task processor test", OnDebugTaskProcessorTestClicked);
		_menu.AddItem("Debug/Serialization test",  OnDebugSerializationTestClicked);
		_menu.AddItem("Debug/Importer test",       OnDebugImporterTestClicked);
		_menu.AddItem("Debug/Show ImGui demo",     OnDebugShowImGuiDemoClicked, null, () => _showImGuiDemoWindow);
		_menu.AddItem("Debug/Test",                OnDebugTestClicked);
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

	private void OnProjectClearResourcesClicked()
	{
		_studioModel.Project.ClearResources();
	}

	private void OnProjectReimportAllClicked()
	{
		_studioModel.Project.ClearResources();
		_studioModel.Project.ImportAll();
	}

	private void OnProjectImportChangedClicked()
	{
		_studioModel.Project.ImportAll();
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

		string json = JsonSerializer.Serialize(templates, new JsonSerializerOptions{IncludeFields = true});
		File.WriteAllText("D:\\Tmp\\serialization_test.json", json);

		List<DefinitionTemplateTest>? deserialized = JsonSerializer.Deserialize<List<DefinitionTemplateTest>>(json, new JsonSerializerOptions {IncludeFields = true});

		int d = 0;
	}

	private void OnDebugImporterTestClicked()
	{
//		ImporterTests.Test();
	}

	private void OnDebugShowImGuiDemoClicked()
	{
		_showImGuiDemoWindow = !_showImGuiDemoWindow;
	}
	
	private void OnDebugTestClicked()
	{
		
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

public class ReferenceTest
{
}

public class ReferenceTestString : ReferenceTest
{
	public string StrValue;
}

public class ReferenceTestInt : ReferenceTest
{
	public int IntValue;
}

public sealed class GenericReference<T> where T : ReferenceTest
{
	public string Guid;
	public string Path;
	public T      Value;
}

public sealed class DefinitionTemplateTest
{
	//--- data begin ---
	public int     IntField;
	public float   FloatField;
	public string  StringField;
	public bool    MyField;
	public string? Empty = null;

	public GenericReference<ReferenceTestInt> Reference = new GenericReference<ReferenceTestInt>() {Guid = "guid", Path = "path", Value = new ReferenceTestInt() {IntValue = 523}};
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