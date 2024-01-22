using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public sealed class ContentDefinitionAsset : Content
{
	private DefinitionAsset? _asset = null;
	public  DefinitionAsset? Asset => _asset;

	public override bool Load(StudioModel model, ProjectNode node)
	{
		_asset = DefinitionAsset.CreateFromFile(node.AbsolutePath, model.MigrationManager);
		return _asset != null;
	}
}