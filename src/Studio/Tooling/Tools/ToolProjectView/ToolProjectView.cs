﻿using ImGuiNET;
using Mine.Framework;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;
using Gui = ImGuiNET.ImGui;

namespace RedHerring.Studio.Tools;

[Tool(ToolName)]
public sealed class ToolProjectView : Tool
{
	public const string ToolName = FontAwesome6.FolderTree + " Project view";
	
	private const ImGuiTreeNodeFlags TreeCommonFlags       = ImGuiTreeNodeFlags.SpanAvailWidth;
	private const ImGuiTreeNodeFlags TreeInternalNodeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | TreeCommonFlags;
	private const ImGuiTreeNodeFlags TreeLeafNodeFlags     = ImGuiTreeNodeFlags.Leaf        | ImGuiTreeNodeFlags.NoTreePushOnOpen  | TreeCommonFlags;
	
	protected override string Name => ToolName;

	private Dictionary<ProjectNode, string> _nodeLabels = new();
    
	public ToolProjectView(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
	}

	public override void Update(out bool finished)
	{
		finished = UpdateUI();
	}

	private bool UpdateUI()
	{
		bool isOpen = true;
		if (Gui.Begin(NameId, ref isOpen))
		{
			UpdateFolder(StudioModel.Project.AssetsFolder);
			UpdateFolder(StudioModel.Project.ScriptsGameLibraryFolder);
			Gui.End();
		}

		if (!isOpen)
		{
			// called in the frame when the window is closed
			_nodeLabels.Clear();
		}

		return !isOpen;
	}

	private void UpdateFolder(ProjectFolderNode? node)
	{
		if (node is null)
		{
			return;
		}

		bool nodeExpanded = UpdateNode(node, TreeInternalNodeFlags);
		if (nodeExpanded)
		{
			foreach (ProjectNode child in node.Children)
			{
				if (child is ProjectFolderNode folder)
				{
					UpdateFolder(folder);
				}
				else if (child is ProjectAssetFileNode or ProjectScriptFileNode)
				{
					UpdateFile(child);
				}
			}
            
			Gui.TreePop();
		}
	}
    
	private void UpdateFile(ProjectNode node)
	{
		UpdateNode(node, TreeLeafNodeFlags);
	}
	
	private bool UpdateNode(ProjectNode node, ImGuiTreeNodeFlags flags)
	{
		if (node.Meta == null)
		{
			return false;
		}

		string id = node.Meta.Guid!;

		if (StudioModel.Selection.IsSelected(id))
		{
			flags |= ImGuiTreeNodeFlags.Selected;
		}
		
		if(!_nodeLabels.TryGetValue(node, out string? label)) // TODO - should react to node.Type change
		{
			// if (node is ProjectFolderNode folder)
			// {
			// 	label = $"{Icon.FolderIconText(folder.Children.Count == 0)} {node.Name}";
			// }
			// else
			// {
			// 	label = $"{Icon.FileIconText(node.Path)} {node.Name}";
			// }
			//label = node.Name;
			label = $"{node.Type.ToIcon()} {node.Name}";
			_nodeLabels.Add(node, label);
		}
		
		bool nodeExpanded = Gui.TreeNodeEx(id, flags, label);

		if (Gui.IsItemClicked() && !Gui.IsItemToggledOpen())
		{
			HandleSelection(id, node);
		}

		return nodeExpanded;
	}

	private void HandleSelection(string id, ProjectNode node)
	{
		if(Gui.GetIO().KeyCtrl)
		{
			StudioModel.Selection.Flip(id, node);
			return;
		}

		if (Gui.GetIO().KeyShift)
		{
			// TODO
			return;
		}

		StudioModel.Selection.DeselectAll();
		StudioModel.Selection.Select(id, node);
	}



}