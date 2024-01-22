using Mine.Studio;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;

namespace RedHerring.Studio.UserInterface;

public class InspectorStudio : Inspector, IInspectorStudio
{
	private         InspectorStudioControlMap _studioControlMap = new();
	public override InspectorControlMap       ControlMap => _studioControlMap;

	private readonly StudioModel  _studioModel;
	private readonly ProjectModel _projectModel;

	private readonly InspectorReferencePopup _referencePopup;
	private          bool                    _requestOpenReferencePopup = false;
	private          StudioReference         _reference                 = null!;
	private          Action<ProjectNode?>    _onReferenceSelected;
	
	public InspectorStudio(ICommandHistory commandHistory, StudioModel studioModel, ProjectModel projectModel) : base(commandHistory)
	{
		_studioModel    = studioModel;
		_projectModel   = projectModel;
		_referencePopup = new InspectorReferencePopup(_studioModel, projectModel, $"{_uniqueId}.referencePopup");
	}

	public void OpenReferencePopup(StudioReference value, Action<ProjectNode?> onSelected)
	{
		_reference                 = value;
		_onReferenceSelected       = onSelected;
		_requestOpenReferencePopup = true;
	}

	public override void Update()
	{
		if (_requestOpenReferencePopup)
		{
			_requestOpenReferencePopup = false;
			_referencePopup.Open(_reference, _onReferenceSelected);
		}

		_referencePopup.Update();
		base.Update();
	}

	public string ProjectNodeGuidToName(string guid)
	{
		ProjectNode? node = _projectModel.FindNodeByGuid(guid);
		return node?.Name ?? "";
	}
}