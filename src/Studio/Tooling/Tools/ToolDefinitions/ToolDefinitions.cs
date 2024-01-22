using System.Numerics;
using ImGuiNET;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Tools;

namespace Mine.Studio;

[Tool(ToolName)]
public sealed class ToolDefinitions : Tool
{
	public const       string ToolName = FontAwesome6.Table + " Definitions";
	protected override string Name => ToolName;

	private readonly string _tabBarId;
	private readonly string _tabDefinitionNameId;
	private readonly string _tabDataNameId;

	private object? _prevSelected = null;

	private readonly ProjectModel _projectModel;
	
	// template editor
	private ProjectScriptFileNode?        _definitionTemplateNode   = null;
	private DefinitionTemplate?           _definitionTemplate       = null;
	private ToolDefinitionTemplateEditor? _definitionTemplateEditor = null;
	
	// asset editor
	private ProjectAssetFileNode?      _definitionAssetNode   = null;
	private DefinitionAsset?           _definitionAsset       = null;
	private ToolDefinitionAssetEditor? _definitionAssetEditor = null;

	public ToolDefinitions(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
		_projectModel        = studioModel.Project;
		_tabBarId            = NameId + ".tabbar";
		_tabDefinitionNameId = $"Definition##{Id}.definition";
		_tabDataNameId       = $"Data##{Id}.data";
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

		_definitionTemplateEditor!.Update();
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

		_definitionAssetEditor!.Update();
	}

	private void UpdateNodesFromSelection()
	{
		// check selection
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

		// get script node
		if (selected is ProjectScriptFileNode scriptFileNode && scriptFileNode.Type == ProjectNodeType.ScriptDefinition)
		{
			if (ReferenceEquals(scriptFileNode, _definitionTemplateNode))
			{
				return;
			}

			_definitionTemplateNode = scriptFileNode;
		}
		else
		{
			_definitionTemplateNode = null;
		}

		// get asset node
		if (selected is ProjectAssetFileNode assetFileNode && assetFileNode.Type == ProjectNodeType.AssetDefinition)
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

		// try to read asset node
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
			_definitionAssetEditor  = new ToolDefinitionAssetEditor(StudioModel.Project, _definitionAssetNode, _definitionAsset, NameId + ".asset_editor");
		}

		// try to read script node
		if (_definitionTemplateNode != null)
		{
			_definitionTemplate = DefinitionTemplate.CreateFromFile(_definitionTemplateNode.AbsolutePath);
			if (_definitionTemplate == null)
			{
				_definitionAsset        = null;
				_definitionTemplateNode = null;
				_definitionAssetNode    = null;
				return;
			}

			_definitionTemplateEditor = new ToolDefinitionTemplateEditor(_projectModel, _definitionTemplateNode, _definitionTemplate, NameId + ".template_editor");
		}
	}
}