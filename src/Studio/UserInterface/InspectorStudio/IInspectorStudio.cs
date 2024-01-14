using Mine.Studio;
using RedHerring.Studio.Models.Project.FileSystem;

namespace RedHerring.Studio.UserInterface;

// studio specific functions used in studio specific controls
public interface IInspectorStudio : IInspector
{
	void   OpenReferencePopup(StudioReference value, Action<ProjectNode?> onSelected);
	bool   UpdateReferencePopup();
	string ProjectNodeGuidToName(string guid);
}