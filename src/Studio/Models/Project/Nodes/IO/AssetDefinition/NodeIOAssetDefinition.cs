using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;

namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetDefinition)]
public sealed class NodeIOAssetDefinition : NodeIO
{
	private DefinitionAsset? _asset = null;
	public  DefinitionAsset? Asset => _asset;
	
	public NodeIOAssetDefinition(ProjectNode owner) : base(owner)
	{
	}

	public override void Load()
	{
		if (_asset != null)
		{
			return;
		}

		_asset = DefinitionAsset.CreateFromFile(Owner.AbsolutePath, StudioGlobals.MigrationManager);
	}

	public override void Save()
	{
		_asset.WriteToFile(Owner.AbsolutePath);
	}

	public override void ClearCache()
	{
		_asset = null;
	}

	public override void Import(string resourcePath)
	{
		Load();
		if (_asset == null)
		{
			return;
		}

		_asset.ImportToResources(resourcePath);
		ClearCache();
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOAssetDefinitionSettings();
	}
}