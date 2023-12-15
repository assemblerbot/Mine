using ImGuiNET;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models.ViewModels.Console;
using RedHerring.Studio.UserInterface.Attributes;

namespace Mine.Studio;

public class CreateScriptDialog
{
	private readonly ObjectDialog _dialog;

	[ReadOnlyInInspector]                  private string _type;
	[ShowInInspector, ReadOnlyInInspector] private string _path = "";
	private                                        string _namespace = "MyNamespace";
	private                                        string _class = "MyClass";

	public CreateScriptDialog()
	{
		_dialog = new ObjectDialog("Create script", new CommandHistory(), this);
	}

	public void Open(string type, string path)
	{
		_type = type;
		_path = path;
		_dialog.Open();
	}

	public void Update()
	{
		_dialog.Update();
	}
	
	[Button("Create!")]
	private void Create()
	{
		ConsoleViewModel.Log($"Creating new script at {_path}", ConsoleItemType.Info);
		// try
		// {
		// 	TemplateUtility.InstantiateTemplate(_targetPath, _name);
		// }
		// catch(Exception e)
		// {
		// 	ConsoleViewModel.Log(e.Message,    ConsoleItemType.Exception);
		// 	ConsoleViewModel.Log(e.StackTrace, ConsoleItemType.Exception);
		// 	return;
		// }

		ImGui.CloseCurrentPopup();
	}
}