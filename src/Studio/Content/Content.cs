using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public abstract class Content
{
	public abstract bool Load(StudioModel model, ProjectNode node);
}