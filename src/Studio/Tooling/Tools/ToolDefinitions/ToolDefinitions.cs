using System.Numerics;
using ImGuiNET;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Tools;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Tool(ToolName)]
public sealed class ToolDefinitions : Tool
{
	public const       string ToolName = FontAwesome6.Table + " Definitions";
	protected override string Name => ToolName;

	private readonly string _tabBarId;
	private readonly string _tabDefinitionNameId;
	private readonly string _tabDataNameId;
	private readonly string _definitionTemplateChildId;
	private readonly string _buttonApplyTemplateChangesNameId;

	private ProjectScriptFileNode?    _definitionTemplateNode         = null;
	private DefinitionTemplate?       _definitionTemplate             = null;
	private Inspector?                _definitionTemplateInspector    = null;
	private CommandHistoryWithChange? _definitionTemplateHistory      = null;
	private string?                   _definitionTemplateErrorMessage = null;
	
	private ProjectAssetFileNode? _definitionAssetNode = null;
	private DefinitionAsset       _definitionAsset     = null;

	public ToolDefinitions(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
		_tabBarId                         = NameId + ".tabbar";
		_tabDefinitionNameId              = $"Definition##{Id}.definition";
		_tabDataNameId                    = $"Data##{Id}.data";
		_definitionTemplateChildId        = $"{Id}.templatechild";
		_buttonApplyTemplateChangesNameId = $"Apply changes##{Id}.applytemplate";
	}

	public override void Update(out bool finished)
	{
		finished = UpdateUI();
	}

	private bool UpdateUI()
	{
		bool isOpen = true;
		if (ImGui.Begin(NameId, ref isOpen))
		{
			UpdateNodesFromSelection();
			
			if (ImGui.BeginTabBar(_tabBarId))
			{
				if (ImGui.BeginTabItem(_tabDefinitionNameId))
				{
					UpdateTemplateEditorUI();
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem(_tabDataNameId))
				{
					UpdateAssetEditorUI();
					ImGui.EndTabItem();
				}
				ImGui.EndTabBar();
			}
			ImGui.End();
		}

		return !isOpen;
	}

	private void UpdateTemplateEditorUI()
	{
		bool wasChange = _definitionTemplateHistory?.WasChange ?? false; 
		if (wasChange)
		{
			ImGui.PushStyleColor(ImGuiCol.Button,        new Vector4(0.2f, 0.5f, 0.2f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.5f, 0.3f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive,  new Vector4(0.4f, 0.5f, 0.4f, 1f));
		}
		else
		{
			ImGui.BeginDisabled();
		}

		if (ImGui.Button(_buttonApplyTemplateChangesNameId))
		{
			if (UpdateTemplateFile())
			{
				_definitionTemplateHistory!.ResetChange();
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

		if (_definitionTemplateErrorMessage != null)
		{
			ImGui.SameLine();
			ImGui.TextColored(new Vector4(1f, 0.5f, 0.5f, 1f), _definitionTemplateErrorMessage);
		}
		
		if (ImGui.BeginChild(_definitionTemplateChildId))
		{
			_definitionTemplateInspector?.Update();
			ImGui.EndChild();
		}
	}

	private void UpdateAssetEditorUI()
	{

		// TODO
	}

	private void UpdateNodesFromSelection()
	{
		object? selected = StudioModel.Selection.GetFirstSelectedTarget();
		if (selected == null)
		{
			_definitionTemplateNode = null;
			_definitionAssetNode    = null;
			return;
		}

		if (selected is ProjectScriptFileNode scriptFileNode)
		{
			if (ReferenceEquals(scriptFileNode, _definitionTemplateNode))
			{
				return;
			}

			_definitionTemplateNode      = scriptFileNode;
			_definitionTemplate          = DefinitionTemplate.CreateFromFile(scriptFileNode.AbsolutePath);
			_definitionTemplateHistory   = new CommandHistoryWithChange();
			_definitionTemplateInspector = new Inspector(_definitionTemplateHistory);
			_definitionTemplateInspector.Init(_definitionTemplate);
			return;
		}

		if (selected is ProjectAssetFileNode assetFileNode)
		{
			if(ReferenceEquals(assetFileNode, _definitionAssetNode))
			{
				return;
			}

			_definitionAssetNode = assetFileNode;
			
			// TODO - parse, setup editor
			return;
		}
	}

	private bool UpdateTemplateFile()
	{
		if (_definitionTemplateNode == null)
		{
			return false;
		}

		_definitionTemplateErrorMessage = _definitionTemplate!.Validate();
		if (_definitionTemplateErrorMessage != null)
		{
			return false;
		}

		_definitionTemplate.Write(_definitionTemplateNode.AbsolutePath); // it is safe to write here?
		return true;
	}
}