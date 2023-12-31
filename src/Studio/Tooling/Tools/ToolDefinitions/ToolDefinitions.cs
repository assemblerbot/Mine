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

	private object?                   _prevSelected                   = null;
	private ProjectScriptFileNode?    _definitionTemplateNode         = null;
	private DefinitionTemplate?       _definitionTemplate             = null;
	private Inspector?                _definitionTemplateInspector    = null;
	private CommandHistoryWithChange? _definitionTemplateHistory      = null;
	private string?                   _definitionTemplateErrorMessage = null;
	
	private ProjectAssetFileNode? _definitionAssetNode = null;
	private DefinitionAsset?      _definitionAsset     = null;

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
		if (_definitionTemplateNode == null || _definitionTemplate == null)
		{
			ImGui.TextColored(new Vector4(1f, 0.5f, 0.5f, 1f), "Select definition template.");
			return;
		}

		ImGui.Text(_definitionTemplateNode.RelativePath);
		
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
		if (_definitionTemplate == null || _definitionAssetNode == null || _definitionAsset == null)
		{
			ImGui.TextColored(new Vector4(1f, 0.5f, 0.5f, 1f), "Select definition asset.");
			return;
		}

		if (_definitionTemplate.Fields.Count == 0)
		{
			ImGui.TextColored(new Vector4(1f, 0.5f, 0.5f, 1f), "Add some fields to definition template.");
			return;
		}

		ImGuiTableFlags flags =
			ImGuiTableFlags.Borders       |
			ImGuiTableFlags.Resizable     |
			ImGuiTableFlags.Sortable      |
			ImGuiTableFlags.Reorderable   |
			ImGuiTableFlags.BordersInnerH |
			ImGuiTableFlags.RowBg         |
			ImGuiTableFlags.ScrollX       |
			ImGuiTableFlags.ScrollY;
		
		
		IReadOnlyList<DefinitionTemplateField?> fields = _definitionTemplate.Fields;
		if (ImGui.BeginTable("table", fields.Count, flags))
		{
			for (int col = 0; col < fields.Count; ++col)
			{
				ImGui.TableSetupColumn(fields[col]?.Name ?? "null");
			}
			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			for (int row = 0; row < 100; ++row)
			{
				ImGui.TableNextRow();
				for (int col = 0; col < fields.Count; ++col)
				{
					ImGui.TableSetColumnIndex(col);
					ImGui.Text("this is some text in the table");
				}
			}

			ImGui.EndTable();
		}
	}

	private void UpdateNodesFromSelection()
	{
		object? selected = StudioModel.Selection.GetFirstSelectedTarget();
		if (selected == _prevSelected)
		{
			return;
		}
		_prevSelected = selected;

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
		}
		else
		{
			_definitionTemplateNode = null;
		}

		if (selected is ProjectAssetFileNode assetFileNode)
		{
			if (ReferenceEquals(assetFileNode, _definitionAssetNode))
			{
				return;
			}

			_definitionAssetNode = assetFileNode;
		}
		else
		{
			_definitionAssetNode = null;
		}

		if(_definitionAssetNode != null)
		{
			DefinitionAsset? asset = DefinitionAsset.CreateFromFile(_definitionAssetNode.AbsolutePath, StudioModel.MigrationManager);
			if (asset == null)
			{
				_definitionAssetNode = null;
				return;
			}

			_definitionAsset = asset;
			
			ProjectScriptFileNode? templateNode = StudioModel.Project.FindScriptNodeByGuid(_definitionAsset.Template.Header.Guid);
			_definitionTemplateNode = templateNode;
		}

		if (_definitionTemplateNode != null)
		{
			_definitionTemplate          = DefinitionTemplate.CreateFromFile(_definitionTemplateNode.AbsolutePath);
			if (_definitionTemplate == null)
			{
				_definitionAsset        = null;
				_definitionTemplateNode = null;
				_definitionAssetNode    = null;
				return;
			}

			_definitionTemplateHistory   = new CommandHistoryWithChange();
			_definitionTemplateInspector = new Inspector(_definitionTemplateHistory);
			_definitionTemplateInspector.Init(_definitionTemplate);
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