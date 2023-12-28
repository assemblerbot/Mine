using ImGuiNET;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.ViewModels.Console;
using RedHerring.Studio.UserInterface.Attributes;

namespace Mine.Studio;

public class CreateDefinitionAssetDialog
{
	private readonly ProjectModel _projectModel;
	private readonly ObjectDialog _dialog;

	[ShowInInspector, ReadOnlyInInspector] private string _path = "";
	[ShowInInspector]                      private string _name = "MyDefinition";

	[ShowInInspector, ValueDropdown("_templates")] private string            _template  = "";
	private                                                List<ProjectScriptFileNode> _templates = new();
	
	private Action<string, string, ProjectScriptFileNode>? _onCreate; // file path, name, template
	
	public CreateDefinitionAssetDialog(ProjectModel projectModel)
	{
		_projectModel = projectModel;
		_dialog       = new ObjectDialog("Create definition", new CommandHistory(), this);
	}

	public void Open(string path, Action<string, string, ProjectScriptFileNode> onCreate)
	{
		InitTemplates();
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
		string filePath = Path.Combine(_projectModel.ProjectSettings.AbsoluteAssetsPath, _path, _name + ".def");
		if (File.Exists(filePath))
		{
			ConsoleViewModel.LogError($"File at '{filePath}' already exist! Cannot create file!");
			// TODO - better UI response
			return;
		}

		ProjectScriptFileNode? selectedTemplate = _templates.FirstOrDefault(template => template.Name == _template);
		if (selectedTemplate == null)
		{
			ConsoleViewModel.LogError("Invalid template selected! Cannot create file!");
			// TODO - better UI response
			return;
		}

		ConsoleViewModel.LogInfo($"Creating new file '{filePath}'");
		try
		{
			_onCreate?.Invoke(filePath, _name, selectedTemplate);
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

	private void InitTemplates()
	{
		_templates.Clear();

		_projectModel.ScriptsFolder!.TraverseRecursive(
			node =>
			{
				if (node.Type == ProjectNodeType.ScriptDefinitionTemplate)
				{
					_templates.Add((ProjectScriptFileNode)node);
				}
			},
			TraverseFlags.Files,
			new CancellationToken()
		);
	}
}