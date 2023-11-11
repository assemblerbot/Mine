using GameToolkit.Framework;
using ImGuiNET;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.Importers;
using RedHerring.Studio.Tools;
using RedHerring.Studio.UserInterface;
using RedHerring.Studio.UserInterface.Dialogs;

namespace GameToolkit.Studio;

public sealed class StudioComponent : Component, IUpdatable
{
	public int GetUpdateOrder() => 0;

	private StudioModel    _studioModel = new();
	private ImporterThread _importerThread;

	private ToolManager _toolManager = new();
	
	#region User Interface
	private readonly DockSpace      _dockSpace       = new();
	private readonly Menu           _menu            = new();
	private readonly StatusBar      _statusBar       = new();
	private          SettingsDialog _projectSettings = null!;
	private          SettingsDialog _studioSettings  = null!;
	private readonly MessageBox     _messageBox      = new();
	#endregion
	
	public override void AfterAddedToScene()
	{
		Engine.Scene.RegisterUpdatable(this);
		
		InitImGui();
		InitMenu();

		_toolManager.Init(_studioModel);

		_projectSettings = new SettingsDialog("Project settings", _studioModel.CommandHistory, _studioModel.Project.ProjectSettings);
		_studioSettings  = new SettingsDialog("Studio settings",  _studioModel.CommandHistory, _studioModel.StudioSettings);
		
		Engine.Scene.Add(new GameObject("Test Object").AddComponent<TestRenderComponent>());

		_statusBar.Message = "Ready";

		// List<int> test   = new() {2, 4, 6, 8, 10, 12, 13};
		// int       index0 = test.FindInsertionIndexBinary(x => x.CompareTo(0));
		// int       index1 = test.FindInsertionIndexBinary(x => x.CompareTo(15));
		// int       index2 = test.FindInsertionIndexBinary(x => x.CompareTo(5));
		// int       index3 = test.FindInsertionIndexBinary(x => x.CompareTo(4));
		// int       index4 = test.FindInsertionIndexBinary(x => x.CompareTo(2));
		// int       index5 = test.FindInsertionIndexBinary(x => x.CompareTo(12));
		//
		// int       d      = 0;
	}
	
	public override void BeforeRemovedFromScene()
	{
		Engine.Scene.UnregisterUpdatable(this);
	}

	public override void Dispose()
	{
		
	}

	public void Update(double timeDelta)
	{
		_dockSpace.Update();
		_menu.Update();
		_statusBar.Update();
		_projectSettings.Update();
		_studioSettings.Update();
		_messageBox.Update();

		_toolManager.Update();
	}
	
	private void InitImGui()
	{
		Engine.Scene.Add(new GameObject("ImGui").AddComponent<ImGuiComponent>());
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
	}
	
	#region Menu
	private void InitMenu()
	{
		_menu.AddItem("File/Open project..", OnOpenProjectClicked);
		_menu.AddItem("File/Exit",           OnExitClicked);

		_menu.AddItem("Edit/Undo",               _studioModel.CommandHistory.Undo);
		_menu.AddItem("Edit/Redo",               _studioModel.CommandHistory.Redo);
		_menu.AddItem("Edit/Project settings..", OnEditProjectSettingsClicked);
		_menu.AddItem("Edit/Studio settings..",  OnEditStudioSettingsClicked);

		_menu.AddItem("View/Project",   OnViewProjectClicked);
		_menu.AddItem("View/Console",   OnViewConsoleClicked);
		_menu.AddItem("View/Inspector", OnViewInspectorClicked);

		_menu.AddItem("Debug/Modal window",        () => ImGui.OpenPopup("MessageBox"));
		_menu.AddItem("Debug/Task processor test", OnDebugTaskProcessorTestClicked);
		_menu.AddItem("Debug/Serialization test",  OnDebugSerializationTestClicked);
		_menu.AddItem("Debug/Importer test",       OnDebugImporterTestClicked);
	}

	private async void OnOpenProjectClicked()
	{
		// DialogResult result = Dialog.FolderPicker();
		// if(!result.IsOk)
		// {
		// 	return;
		// }
  //       
		// await _studioModel.OpenProject(result.Path);
		// _importerThread.Continue();
	}

	private void OnExitClicked()
	{
		Engine.Exit();
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
	}

	private void OnDebugImporterTestClicked()
	{
//		ImporterTests.Test();
	}
	#endregion
}