using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public sealed class ContentDefinitionTemplate : Content
{
	private DefinitionTemplate? _template = null;

	public void Load(StudioModel model, ProjectNode node)
	{
		_template = DefinitionTemplate.CreateFromFile(node.AbsolutePath);
	}
}