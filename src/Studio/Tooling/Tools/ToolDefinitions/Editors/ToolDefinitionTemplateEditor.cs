using System.Numerics;
using ImGuiNET;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

public class ToolDefinitionTemplateEditor
{
	private readonly StudioModel           _studioModel;
	private readonly ProjectScriptFileNode _definitionTemplateNode;
	private readonly DefinitionTemplate    _definitionTemplate;
	private readonly string                _editorUniqueId;

	private readonly InspectorStudio          _inspector;
	private readonly CommandHistoryWithChange _commandHistory;
	private          string?                  _errorMessage = null;

	public ToolDefinitionTemplateEditor(StudioModel studioModel, ProjectModel projectModel, ProjectScriptFileNode definitionTemplateNode, DefinitionTemplate definitionTemplate, string editorUniqueId)
	{
		_studioModel            = studioModel;
		_definitionTemplateNode = definitionTemplateNode;
		_definitionTemplate     = definitionTemplate;
		_editorUniqueId         = editorUniqueId;

		_commandHistory = new CommandHistoryWithChange();
		_inspector      = new InspectorStudio(_commandHistory, studioModel, projectModel);
		_inspector.Init(_definitionTemplate);
	}

	public void Update()
	{
		ImGui.PushID(_editorUniqueId);
		
		ImGui.Text(_definitionTemplateNode.RelativePath);
		
		//--- buttons ---
		bool wasChange = _commandHistory.WasChange; 
		if (wasChange)
		{
			// TODO - move to style
			ImGui.PushStyleColor(ImGuiCol.Button,        new Vector4(0.2f, 0.5f, 0.2f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.5f, 0.3f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive,  new Vector4(0.4f, 0.5f, 0.4f, 1f));
		}
		else
		{
			ImGui.BeginDisabled();
		}

		if (ImGui.Button("Apply changes"))
		{
			if (WriteToFile())
			{
				_commandHistory.ResetChange();
			}
		}

		if (wasChange)
		{
			ImGui.PopStyleColor(3);
		}
		else
		{
			ImGui.EndDisabled();
		}

		ImGui.SameLine();
		if (_commandHistory.CanUndo)
		{
			if (ImGui.Button(FontAwesome6.RotateLeft))
			{
				_commandHistory.Undo();
			}
		}
		else
		{
			ImGui.BeginDisabled();
			ImGui.Button(FontAwesome6.RotateLeft);
			ImGui.EndDisabled();
		}

		ImGui.SameLine();
		if (_commandHistory.CanRedo)
		{
			if (ImGui.Button(FontAwesome6.RotateRight))
			{
				_commandHistory.Redo();
			}
		}
		else
		{
			ImGui.BeginDisabled();
			ImGui.Button(FontAwesome6.RotateRight);
			ImGui.EndDisabled();
		}
		
		if (_errorMessage != null)
		{
			ImGui.SameLine();
			ImGui.TextColored(new Vector4(1f, 0.5f, 0.5f, 1f), _errorMessage);
		}
		
		//--- inspector
		if (ImGui.BeginChild("inspector"))
		{
			_inspector?.Update();
			ImGui.EndChild();
		}

		ImGui.PopID();
	}

	private bool WriteToFile()
	{
		_errorMessage = _definitionTemplate!.Validate();
		if (_errorMessage != null)
		{
			return false;
		}

		_definitionTemplate.WriteToFile(_definitionTemplateNode.AbsolutePath, _definitionTemplateNode.Meta?.Guid, _studioModel.Project); // it is safe to write here?
		return true;
	}
}