﻿using ImGuiNET;
using Mine.ImGuiPlugin;
using NativeFileDialogSharp;
using Gui = ImGuiNET.ImGui;

namespace Mine.Studio;

[Tool(ToolName)]
public sealed class ToolProjectView : Tool
{
	public const string ToolName = FontAwesome6.FolderTree + " Project view";
	
	private const ImGuiTreeNodeFlags TreeCommonFlags       = ImGuiTreeNodeFlags.SpanAvailWidth;
	private const ImGuiTreeNodeFlags TreeInternalNodeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | TreeCommonFlags;
	private const ImGuiTreeNodeFlags TreeLeafNodeFlags     = ImGuiTreeNodeFlags.Leaf        | ImGuiTreeNodeFlags.NoTreePushOnOpen  | TreeCommonFlags;
	
	protected override string Name => ToolName;

	private readonly ProjectModel                                       _projectModel;
	private readonly Dictionary<(ProjectNode, ProjectNodeKind), string> _nodeLabels = new();
	
	private readonly Menu         _contextMenu            = new(MenuStyle.ContextMenu);
	private          ProjectNode? _contextMenuActivatedAt = null;

	private readonly CreateScriptDialog          _createScriptDialog;
	private readonly CreateDefinitionAssetDialog _createDefinitionAssetDialog;
    
	public ToolProjectView(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
		_projectModel = studioModel.Project;
		CreateContextMenu();
		_createScriptDialog          = new CreateScriptDialog(studioModel.Project);
		_createDefinitionAssetDialog = new CreateDefinitionAssetDialog(studioModel.Project);
	}

	public override void Update(out bool finished)
	{
		UpdateDialogs();
		finished = UpdateUI();
		_contextMenu.InvokeClickActions();
	}

	private void UpdateDialogs()
	{
		_createScriptDialog.Update();
		_createDefinitionAssetDialog.Update();
	}

	private bool UpdateUI()
	{
		bool isOpen = true;
		if (Gui.Begin(NameId, ref isOpen, ImGuiWindowFlags.HorizontalScrollbar))
		{
			lock (StudioModel.Project.ProjectTreeLock)
			{
				UpdateFolder(StudioModel.Project.AssetsFolder);
				UpdateFolder(StudioModel.Project.ScriptsFolder);
			}

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
		
		if(!_nodeLabels.TryGetValue((node, node.Kind), out string? label))
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
			label = $"{node.Kind.ToIcon()} {node.Name}";
			_nodeLabels.Add((node, node.Kind), label);
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
		// _contextMenu.AddItem("Create/DefinitionTemplate", OnCreateDefinitionTemplate, CanCreateScript);
		// _contextMenu.AddItem("Create/DefinitionAsset",    OnCreateDefinitionAsset,    CanCreateAsset);
		_contextMenu.AddItem("Create/Prefab", OnCreatePrefab, CanCreatePrefab);

		_contextMenu.AddItem("Refresh meta", OnContextMenuRefresh, IsAnythingSelected);
		_contextMenu.AddItem("Reimport", OnContextMenuReimport, IsAnythingSelected);
		_contextMenu.AddSeparator("");
		
		_contextMenu.AddItem("Edit/Rename", OnContextMenuEditRename, IsChangeOfContextItemPossible);
		_contextMenu.AddItem("Edit/Copy",   OnContextMenuEditCopy,   IsChangeOfContextItemPossible);
		_contextMenu.AddItem("Edit/Paste",  OnContextMenuEditPaste,  IsCreationUnderContextItemPossible);
		_contextMenu.AddItem("Edit/Cut",    OnContextMenuEditCut,    IsChangeOfContextItemPossible);
		_contextMenu.AddItem("Edit/Delete", OnContextMenuEditDelete, IsChangeOfContextItemPossible);
	}

	#region Create
	// private void OnCreateDefinitionTemplate()
	// {
	// 	_createScriptDialog.Open("Definition template", _contextMenuActivatedAt!.RelativeDirectoryPath, OnCreateDefinitionTemplateFile);
	// }
	// private void OnCreateDefinitionTemplateFile(string path, string namespaceName, string className)
	// {
	// 	StudioModel.Project.PauseWatchers();
	//
	// 	try
	// 	{
	// 		DefinitionTemplate template = new(namespaceName, className);
	// 		template.WriteToFile(path, null, StudioModel.Project);
	// 	}
	// 	catch (Exception e)
	// 	{
	// 		ConsoleViewModel.LogException(e.ToString());
	// 	}
	//
	// 	StudioModel.Project.ResumeWatchers();
	// }
	//
	// private void OnCreateDefinitionAsset()
	// {
	// 	_createDefinitionAssetDialog.Open(_contextMenuActivatedAt!.RelativeDirectoryPath, OnCreateDefinitionAssetFile);
	// }
	//
	// private void OnCreateDefinitionAssetFile(string path, string name, ProjectScriptFileNode templateFileNode)
	// {
	// 	StudioModel.Project.PauseWatchers();
	// 	
	// 	try
	// 	{
	// 		DefinitionTemplate? template = DefinitionTemplate.CreateFromFile(templateFileNode.AbsolutePath, StudioModel.Project, false);
	// 		if (template == null)
	// 		{
	// 			ConsoleViewModel.LogError($"Cannot read template file: {templateFileNode.AbsolutePath}");
	// 		}
	// 		else
	// 		{
	// 			DefinitionAsset asset = new (template);
	// 			asset.WriteToFile(path);
	// 		}
	// 	}
	// 	catch (Exception e)
	// 	{
	// 		ConsoleViewModel.LogException(e.ToString());
	// 	}
	//
	// 	StudioModel.Project.ResumeWatchers();
	// }

	private void OnCreatePrefab()
	{
		DialogResult result = Dialog.FileSave("cs", StudioModel.Project.ScriptsFolder!.AbsolutePath);
		if(!result.IsOk)
		{
			return;
		}

		string path = result.Path;
		if (Path.GetExtension(path) != ".cs")
		{
			path += ".cs";
		}

		ConsoleViewModel.LogInfo($"Writing prefab to file: '{path}'");
		StudioModel.Project.PauseWatchers();
		SceneImporterPrefab.CreatePrefab(path, _contextMenuActivatedAt!);
		StudioModel.Project.ResumeWatchers();
	}
	#endregion
	
	#region Refresh
	private void OnContextMenuRefresh()
	{
		_contextMenuActivatedAt!.RefreshMetaFile();
	}

	private void OnContextMenuReimport()
	{
		StudioModel.Project.Import(_contextMenuActivatedAt!, true);
	}
	#endregion

	#region Edit
	private void OnContextMenuEditRename()
	{
		// TODO
		//ProjectNode? node = StudioModel.Project.AssetsFolder.FindNode("Test/New2\\moonshades.txt");
		//ConsoleViewModel.LogInfo(node == null ? "Not found" : "Found");
	}

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
		return _contextMenuActivatedAt is not null && _contextMenuActivatedAt is not ProjectRootNode;
	}

	private bool IsCreationUnderContextItemPossible()
	{
		return _contextMenuActivatedAt is not null;
	}

	private bool CanCreateScript()
	{
		return _contextMenuActivatedAt is not null && _contextMenuActivatedAt.Kind.IsScriptsRelated();
	}
	
	private bool CanCreateAsset()
	{
		return _contextMenuActivatedAt is not null && _contextMenuActivatedAt.Kind.IsAssetsRelated();
	}

	private bool CanCreatePrefab()
	{
		return _contextMenuActivatedAt is not null && _contextMenuActivatedAt.Kind == ProjectNodeKind.AssetScene;
	}

	private bool IsAnythingSelected()
	{
		return _contextMenuActivatedAt is not null;
	}

	#endregion
	
	#endregion
}