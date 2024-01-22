using ImGuiNET;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public class InspectorReferencePopup
{
	private readonly StudioModel  _studioModel;
	private readonly ProjectModel _projectModel;
	private          string       _id;

	private List<ProjectNode>     _relevantNodes = new();
	private Action<ProjectNode?>? _onSelected;
	
	public InspectorReferencePopup(StudioModel studioModel, ProjectModel projectModel, string id)
	{
		_studioModel  = studioModel;
		_projectModel = projectModel;
		_id           = id;
	}

	public void Open(StudioReference value, Action<ProjectNode?> onSelected)
	{
		_onSelected = onSelected;
		RefreshRelevantNodes(_studioModel, value);
		ImGui.OpenPopup(_id);
	}

	public bool Update()
	{
		if (ImGui.BeginPopup(_id))
		{
			// TODO - filter
			
			if (ImGui.Button("null"))
			{
				_onSelected?.Invoke(null);
				ImGui.CloseCurrentPopup();
			}

			for (int i = 0; i < _relevantNodes.Count; ++i)
			{
				ImGui.PushID(i);
				if (ImGui.Button(_relevantNodes[i].Name))
				{
					_onSelected?.Invoke(_relevantNodes[i]);
					ImGui.CloseCurrentPopup();
				}
				ImGui.PopID();
			}

			ImGui.EndPopup();
		}
		return false;
	}

	private void RefreshRelevantNodes(StudioModel studioModel, StudioReference reference)
	{
		_relevantNodes.Clear();

		_projectModel.AssetsFolder?.TraverseRecursive(
			node =>
			{
				if (reference.CanAcceptNode(_studioModel, node))
				{
					_relevantNodes.Add(node);
				}
			},
			TraverseFlags.Files | TraverseFlags.Directories,
			new CancellationToken()
		);
		
		_projectModel.ScriptsFolder?.TraverseRecursive(
			node => 
			{
				if (reference.CanAcceptNode(studioModel, node))
				{
					_relevantNodes.Add(node);
				}
			},
			TraverseFlags.Files | TraverseFlags.Directories,
			new CancellationToken()
		);
	}
}