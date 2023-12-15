using ImGuiNET;
using Mine.ImGuiPlugin;
using Mine.Studio;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.UserInterface;
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

	private readonly Dictionary<(ProjectNode, ProjectNodeType), string> _nodeLabels = new();

	private readonly Menu         _contextMenu            = new(MenuStyle.ContextMenu);
	private          ProjectNode? _contextMenuActivatedAt = null;

	private readonly CreateScriptDialog _createScriptDialog = new();
    
	public ToolProjectView(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
		CreateContextMenu();
	}

	public override void Update(out bool finished)
	{
		UpdateDialogs();
		finished = UpdateUI();
	}

	private void UpdateDialogs()
	{
		_createScriptDialog.Update();
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
		
		if(!_nodeLabels.TryGetValue((node, node.Type), out string? label))
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
			_nodeLabels.Add((node, node.Type), label);
		}
		
		bool nodeExpanded = Gui.TreeNodeEx(id, flags, label);

		if (Gui.BeginPopupContextItem(id))
		{
			_contextMenuActivatedAt = node;
			_contextMenu.Update();
			Gui.EndPopup();
		}

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
	
	#region Context menu
	private void CreateContextMenu()
	{
		_contextMenu.AddItem("Create/DefinitionTemplate", OnCreateDefinitionTemplate, CanCreateScript);
		_contextMenu.AddItem("Create/DefinitionAsset", OnCreateDefinitionAsset, CanCreateAsset);
		
		_contextMenu.AddItem("Edit/Copy",   OnContextMenuEditCopy,   IsChangeOfContextItemPossible);
		_contextMenu.AddItem("Edit/Paste",  OnContextMenuEditPaste,  IsCreationUnderContextItemPossible);
		_contextMenu.AddItem("Edit/Cut",    OnContextMenuEditCut,    IsChangeOfContextItemPossible);
		_contextMenu.AddItem("Edit/Delete", OnContextMenuEditDelete, IsChangeOfContextItemPossible);
	}

	#region Create
	private void OnCreateDefinitionTemplate()
	{
		_createScriptDialog.Open("Definition template", _contextMenuActivatedAt!.RelativePath);
	}

	private void OnCreateDefinitionAsset()
	{
		// TODO
	}

	#endregion

	#region Edit
	private void OnContextMenuEditCopy()
	{
		// TODO
	}

	private void OnContextMenuEditPaste()
	{
		// TODO
	}

	private void OnContextMenuEditCut()
	{
		// TODO
	}

	private void OnContextMenuEditDelete()
	{
		// TODO
	}
	#endregion
	
	#region Checks
	private bool IsChangeOfContextItemPossible()
	{
		return _contextMenuActivatedAt != null && _contextMenuActivatedAt is not ProjectRootNode;
	}

	private bool IsCreationUnderContextItemPossible()
	{
		return _contextMenuActivatedAt != null;
	}

	private bool CanCreateScript()
	{
		return _contextMenuActivatedAt != null && _contextMenuActivatedAt.Type.IsScriptsRelated();
	}
	
	private bool CanCreateAsset()
	{
		return _contextMenuActivatedAt != null && _contextMenuActivatedAt.Type.IsAssetsRelated();
	}
	#endregion
	
	#endregion
}