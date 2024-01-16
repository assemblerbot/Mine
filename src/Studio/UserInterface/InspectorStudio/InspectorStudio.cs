using Mine.Studio;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;

namespace RedHerring.Studio.UserInterface;

public class InspectorStudio : Inspector, IInspectorStudio
{
	private readonly ProjectModel _projectModel;
	
	public InspectorStudio(ICommandHistory commandHistory, ProjectModel projectModel) : base(commandHistory)
	{
		_projectModel = projectModel;
	}

	public void OpenReferencePopup(StudioReference value, Action<ProjectNode?> onSelected)
	{
		throw new NotImplementedException();
	}

	public bool   UpdateReferencePopup()
	{
		throw new NotImplementedException();
	}

	public string ProjectNodeGuidToName(string guid)
	{
		throw new NotImplementedException();
	}
}