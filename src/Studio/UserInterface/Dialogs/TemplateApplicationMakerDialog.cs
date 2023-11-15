using ImGuiNET;
using NativeFileDialogSharp;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models.ViewModels.Console;
using RedHerring.Studio.UserInterface.Attributes;

namespace Mine.Studio;

public sealed class TemplateApplicationMakerDialog
{
	public int GetUpdateOrder() => 0;

	private readonly ObjectDialog _dialog;
	
	[ShowInInspector, OnCommitValue(nameof(UpdateTargetPath))] private string _name = "";
	[ShowInInspector, ReadOnlyInInspector]                     private string _path = "";

	[Button("Change path..")]
	private void Browse()
	{
		DialogResult result = Dialog.FolderPicker();
		if(!result.IsOk)
		{
			return;
		}

		_path = result.Path;
		UpdateTargetPath();
	}

	[ShowInInspector, ReadOnlyInInspector] private string _targetPath = "";
	[Button("Create!")]
	private void Create()
	{
		ConsoleViewModel.Log($"Creating new application at {_targetPath}", ConsoleItemType.Info);
		try
		{
			TemplateUtility.InstantiateTemplate(_targetPath, _name);
		}
		catch(Exception e)
		{
			ConsoleViewModel.Log(e.Message, ConsoleItemType.Exception);
			ConsoleViewModel.Log(e.StackTrace, ConsoleItemType.Exception);
			return;
		}

		// TODO - open created project
		
		ImGui.CloseCurrentPopup();
	}

	private void UpdateTargetPath()
	{
		_targetPath = Path.Combine(_path, _name);
	}

	public TemplateApplicationMakerDialog()
	{
		_dialog = new ObjectDialog("Create new application", new CommandHistory(), this);
	}
	
	public void Open()
	{
		_dialog.Open();
	}

	public void Update()
	{
		_dialog.Update();
	}
}