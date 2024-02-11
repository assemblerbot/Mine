using CommandProcessorPlugin;
using ImGuiNET;

namespace Mine.Studio;

public class CreateScriptDialog
{
	private readonly ProjectModel _projectModel;
	private readonly ObjectDialog _dialog;

	[ReadOnlyInInspector]                  private string _type      = "";
	[ShowInInspector, ReadOnlyInInspector] private string _path      = "";
	[ShowInInspector]                      private string _namespace = "MyNamespace";
	[ShowInInspector]                      private string _class     = "MyClass";

	private Action<string, string, string>? _onCreate; // file path, namespace, class name

	public CreateScriptDialog(ProjectModel projectModel)
	{
		_projectModel = projectModel;
		_dialog       = new ObjectDialog("Create script", new CommandHistory(), this);
	}

	public void Open(string type, string path, Action<string, string, string> onCreate)
	{
		_type     = type;
		_path     = path;
		_onCreate = onCreate;
		_dialog.Open();
	}

	public void Update()
	{
		_dialog.Update();
	}
	
	[Button("Create!")]
	private void Create()
	{
		string filePath = Path.Combine(_projectModel.ProjectSettings.AbsoluteScriptsPath, _path, _class + ".cs");
		if (File.Exists(filePath))
		{
			ConsoleViewModel.LogError($"File at '{filePath}' already exist! Cannot create file!");
			// TODO - better UI response
			return;
		}

		ConsoleViewModel.LogInfo($"Creating new script '{filePath}'");
		try
		{
			_onCreate?.Invoke(filePath, _namespace, _class);
			ConsoleViewModel.LogInfo("DONE");
		}
		catch(Exception e)
		{
			ConsoleViewModel.LogException(e.Message   );
			ConsoleViewModel.LogException(e.StackTrace);
			return;
		}

		ImGui.CloseCurrentPopup();
	}
}