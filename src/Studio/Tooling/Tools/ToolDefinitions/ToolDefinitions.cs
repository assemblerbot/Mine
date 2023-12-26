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

	private ProjectScriptFileNode? _definitionTemplateNode      = null;
	private DefinitionTemplate?    _definitionTemplate          = null;
	private Inspector?             _definitionTemplateInspector = null;
	
	private ProjectAssetFileNode? _definitionAssetNode = null;
	private DefinitionAsset       _definitionAsset     = null;
	
	public ToolDefinitions(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
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
					_definitionTemplateInspector?.Update();
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem(_tabDataNameId))
				{
					
					ImGui.EndTabItem();
				}
				ImGui.EndTabBar();
			}

			// TODO
			ImGui.End();
		}

		return !isOpen;
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
			_definitionTemplateInspector = new Inspector(new CommandHistory());
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
}