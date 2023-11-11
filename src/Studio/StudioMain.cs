using GameToolkit.Framework;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.Importers;
using RedHerring.Studio.UserInterface;
using RedHerring.Studio.UserInterface.Dialogs;

namespace GameToolkit.Studio;

public sealed class StudioMain
{
	private StudioModel    _studioModel = new();
	private ImporterThread _importerThread;
	
	#region User Interface
	private readonly DockSpace      _dockSpace       = new();
	private readonly Menu           _menu            = new();
	private readonly StatusBar      _statusBar       = new();
	private          SettingsDialog _projectSettings = null!;
	private          SettingsDialog _studioSettings  = null!;
	private readonly MessageBox     _messageBox      = new();
	#endregion
	
	public void OnLoad()
	{
		// initialize scene
		GameObject imguiGameObject = new GameObject("ImGui").AddComponent(new ImGuiComponent());
		Engine.Scene.Add(imguiGameObject);
		imguiGameObject.CallOnComponentsInHierarchy<ImGuiComponent>(x => x.Init());
		
		GameObject testGameObject = new GameObject("Test Object").AddComponent(new TestRenderComponent());
		Engine.Scene.Add(testGameObject);
		testGameObject.CallOnComponentsInHierarchy<TestRenderComponent>(x => x.Init());
	}

	public void OnExit()
	{
		
	}
}