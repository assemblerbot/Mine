using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public sealed class ContentEmpty : Content
{
	public override bool Load(StudioModel model, ProjectNode node)
	{
		return true;
	}
}