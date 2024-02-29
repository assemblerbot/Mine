using System.Numerics;
using Assimp;
using ImGuiNET;
using Migration;
using Mine.Framework;
using Mine.ImGuiPlugin;
using NativeFileDialogSharp;
using Scene = Mine.Framework.Scene;

namespace Mine.Studio;

public sealed class StudioComponent : Component, IUpdatable
{
	public const string Title = "MINE Studio";

	public int UpdateOrder => 0;

	private readonly StudioModel _studioModel = new();

	private readonly ToolManager _toolManager = new();
	
	#region User Interface
	private readonly DockSpace               _dockSpace        = new();
	private readonly Menu                    _menu             = new(MenuStyle.MainMenu);
	private readonly StatusBar               _statusBar        = new();
	private          ObjectDialog?           _projectSettings  = null;
	private          ObjectDialog            _studioSettings   = null!;
	private          NewProjectDialog        _newProjectDialog = null!;
	private readonly MessageBox              _messageBox       = new();
	private readonly StatusBarMessageHandler _statusBarMessageHandler;
	private          bool                    _showImGuiDemoWindow = false;
	#endregion

	public StudioComponent()
	{
		_statusBarMessageHandler = new StatusBarMessageHandler(_statusBar, _studioModel);
		_studioModel.EventAggregator.Register<ProjectModel.OpenedEvent>(OnProjectOpened);
		_studioModel.EventAggregator.Register<ProjectModel.ClosedEvent>(OnProjectClosed);
	}

	public override void AfterAddedToWorld()
	{
		Engine.World.RegisterUpdatable(this);
		Engine.Instance.OnFocusChanged += OnFocusChanged;
		
		InitImGui();
		InitMenu();

		_toolManager.Init(_studioModel);

		//_projectSettings  = new ObjectDialog("Project settings", _studioModel.CommandHistory, _studioModel.Project.ProjectSettings);
		_studioSettings   = new ObjectDialog("Studio settings",  _studioModel.CommandHistory, _studioModel.StudioSettings);
		_newProjectDialog = new NewProjectDialog(_studioModel);

		LoadSettings();

		CreateDebugEntities();
	}

	public override void BeforeRemovedFromWorld()
	{
		SaveSettings();
		
		_studioModel.Close();
		Engine.World.UnregisterUpdatable(this);
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
		_projectSettings?.Update();
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
		Engine.World.Add(new Entity("ImGui").AddComponent<ImGuiComponent>().Entity);
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
	}
	
	#region Event handlers
	private void OnProjectOpened(ProjectModel.OpenedEvent obj)
	{
		Engine.Window.Title = $"{Title} - {_studioModel.Project.ProjectSettings.ProjectFolderPath}";
	}

	private void OnProjectClosed(ProjectModel.ClosedEvent obj)
	{
		Engine.Window.Title = Title;
	}
	#endregion
	
	#region Menu
	private void InitMenu()
	{
		_menu.AddItem("File/New project..",  OnNewProjectClicked);
		_menu.AddItem("File/Open project..", OnOpenProjectClicked);
		_menu.AddItem("File/Exit",           OnExitClicked);

		_menu.AddItem("Edit/Undo",               _studioModel.CommandHistory.Undo);
		_menu.AddItem("Edit/Redo",               _studioModel.CommandHistory.Redo);
		_menu.AddItem("Edit/Project settings..", OnEditProjectSettingsClicked, () => _studioModel.Project.IsOpened);
		_menu.AddItem("Edit/Studio settings..",  OnEditStudioSettingsClicked);

		_menu.AddItem("View/Project",     OnViewProjectClicked);
		_menu.AddItem("View/Console",     OnViewConsoleClicked);
		_menu.AddItem("View/Inspector",   OnViewInspectorClicked);
		_menu.AddItem("View/Plugins",     OnViewPluginsClicked);

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
		_projectSettings = new ObjectDialog("Project settings", _studioModel.CommandHistory, _studioModel.Project.ProjectSettings);
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

	public static SceneReference Suzanne = new(@"Test/suzanne.fbx.scene"); // that will be generated
	private void CreateDebugEntities()
	{
		// instantiate prefab
		//Entity suzanne = Engine.World.Instantiate(Suzanne);
		
		Engine.World.Add(new Entity("Test Object").AddComponent<TestRenderComponent>().Entity);

		// mesh
		{
			Entity entity = new Entity("Suzanne");
			entity.AddComponent(new MeshComponent(Suzanne, 0, null));

			// TODO material
			
			Engine.World.Add(entity);
		}
		
		// camera
		{
			Entity          entity          = new Entity("Camera");
			CameraComponent cameraComponent = entity.AddComponent<CameraComponent>();
			Engine.World.Add(entity);

			entity.LocalPosition = new Point3Float(0, -10, 0);
		}

/*
		Entity e1 = new Entity();
		Entity e2 = new Entity();
		Entity e3 = new Entity();

		e2.SetParent(e1);
		e3.SetParent(e2);

		e2.LocalPosition = new Point3Float(1, 2, 3);
		e2.LocalRotation = QuaternionFloat.CreateFromYawPitchRoll(2, 1, 3);
		e2.LocalScale    = new Vector3Float(2, 3, 4);

		e1.LocalPosition = new Point3Float(4, 5, 6);
		e1.LocalRotation = QuaternionFloat.CreateFromYawPitchRoll(5, 4, 6);
		e1.LocalScale    = new Vector3Float(7, 8, 9);

		PrintMatrix("e1", e1.LocalToWorldMatrix);
		PrintMatrix("e2", e2.LocalToWorldMatrix);
		PrintMatrix("e3", e3.LocalToWorldMatrix);

		PrintMatrix("e1i", e1.WorldToLocalMatrix);
		PrintMatrix("e2i", e2.WorldToLocalMatrix);
		PrintMatrix("e3i", e3.WorldToLocalMatrix);
		*/
	}

	private void PrintMatrix(string name, Matrix4x4Float matrix)
	{
		using FileStream   stream = File.Open("d:\\Tmp\\transform_mine.txt", FileMode.Append);
		using StreamWriter writer = new StreamWriter(stream);

		writer.WriteLine(name);
		writer.WriteLine($"{matrix.M11,7:F3} | {matrix.M12,7:F3} | {matrix.M13,7:F3} | {matrix.M14,7:F3}");
		writer.WriteLine($"{matrix.M21,7:F3} | {matrix.M22,7:F3} | {matrix.M23,7:F3} | {matrix.M24,7:F3}");
		writer.WriteLine($"{matrix.M31,7:F3} | {matrix.M32,7:F3} | {matrix.M33,7:F3} | {matrix.M34,7:F3}");
		writer.WriteLine($"{matrix.M41,7:F3} | {matrix.M42,7:F3} | {matrix.M43,7:F3} | {matrix.M44,7:F3}");
		writer.WriteLine();
	}
	
	private struct TestVertex
	{
		public const int     Size = 3 * 4 + 3 * 4;
		public       Vector3 Position;
		public       Color3D Color;
	}

	private unsafe void OnDebugSerializationTestClicked()
	{
//		SerializationTests.Test();
		// List<DefinitionTemplateTest> templates = new() {new DefinitionTemplateTest(), new DefinitionTemplateTest(), new DefinitionTemplateTest()};
		//
		// string json = JsonSerializer.Serialize(templates, new JsonSerializerOptions{IncludeFields = true});
		// File.WriteAllText("D:\\Tmp\\serialization_test.json", json);
		//
		// List<DefinitionTemplateTest>? deserialized = JsonSerializer.Deserialize<List<DefinitionTemplateTest>>(json, new JsonSerializerOptions {IncludeFields = true});

		// TestVertex[] vertices = new[]
		//                         {
		// 	                        new TestVertex{Position = new Vector3(1, 2, 3),Color =new Color3D(0.1f, 0.2f, 0.3f)},
		// 	                        new TestVertex{Position = new Vector3(4, 5, 6),Color =new Color3D(0.4f, 0.5f, 0.6f)},
		// 	                        new TestVertex{Position = new Vector3(7, 8, 9),Color =new Color3D(0.7f, 0.8f, 0.9f)},
		//                         };
		//
		// byte[] bytes;
		// fixed (TestVertex* pVertices = vertices)
		// {
		// 	//bytes = (byte*) pVertices;
		// }
		//
		// using FileStream stream = new("D:\\Tmp\\serialization_test.bin", FileMode.Create);
		// using BinaryWriter     writer = new(stream);

		TestData data = new TestData();
		data.Init();

		byte[] bytes = MigrationSerializer.Serialize(data, SerializedDataFormat.JSON, StudioGlobals.Assembly);
		//byte[] bytes = SerializationUtility.SerializeValue(data, DataFormat.JSON);
		File.WriteAllBytes("c:\\tmp\\test.json", bytes);
		
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


	private enum TestByteEnum : byte
	{
		Abc = 1,
		Def = 2,
	}

	private void OnDebugTestClicked()
	{
		TestByteEnum b = TestByteEnum.Def;
		int          t = (int) b;
		int          d = 0;
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

//--------------------------
public interface ReferenceTest
{
}

[MigratableInterface(typeof(ReferenceTest))]
public interface IReferenceTestMigratable
{
}

//--------------------------
[Serializable, SerializedClassId("reference-test-string")]
public class ReferenceTestString : ReferenceTest
{
	public string StrValue = "";
}

[MigratableInterface(typeof(ReferenceTestString))]
public interface IReferenceTestStringMigratable : IReferenceTestMigratable;

[Serializable, LatestVersion(typeof(ReferenceTestString))]
public class ReferenceTestString_000 : IReferenceTestStringMigratable
{
	public string StrValue;
}

//--------------------------
[Serializable, SerializedClassId("reference-test-int")]
public class ReferenceTestInt : ReferenceTest
{
	public int IntValue = 0;
}

[MigratableInterface(typeof(ReferenceTestInt))]
public interface IReferenceTestIntMigratable : IReferenceTestMigratable;

[Serializable, LatestVersion(typeof(ReferenceTestInt))]
public class ReferenceTestInt_000 : IReferenceTestIntMigratable
{
	public string IntValue;
}

//--------------------------
[Serializable, SerializedClassId("generic-reference-test")]
public sealed class GenericReferenceTest<T> where T : ReferenceTest
{
	public string? Guid = null;
	[NonSerialized] public T?      Data = default(T);
}

[MigratableInterface(typeof(GenericReferenceTest<>))]
public interface IGenericReferenceTestMigratable;

[Serializable, LatestVersion(typeof(GenericReferenceTest<>))]
public class GenericReferenceTest_000 : IGenericReferenceTestMigratable
{
	public string? Guid;
}

//--------------------------
[Serializable, SerializedClassId("test-data")]
public class TestData
{
	public GenericReferenceTest<ReferenceTestInt>    IntReference = new();
	public GenericReferenceTest<ReferenceTestString> StringReference = new();

	public void Init()
	{
		IntReference    = new GenericReferenceTest<ReferenceTestInt> {Guid    = "1234-1234-1234", Data = new ReferenceTestInt {IntValue    = 1234}};
		StringReference = new GenericReferenceTest<ReferenceTestString> {Guid = "55678-5678-5678", Data = new ReferenceTestString {StrValue = "asdf"}};
	}
}

[MigratableInterface(typeof(TestData))]
public interface ITestDataMigratable;
    
[Serializable, LatestVersion(typeof(TestData))]
public class GenericData_000 : ITestDataMigratable
{
	public IGenericReferenceTestMigratable IntReference;
	public IGenericReferenceTestMigratable StringReference;
}
