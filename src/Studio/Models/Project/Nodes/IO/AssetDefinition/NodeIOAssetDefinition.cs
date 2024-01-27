using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;

namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetDefinition)]
public sealed class NodeIOAssetDefinition : NodeIO<DefinitionAsset>
{
	private DefinitionAsset? _asset = null;
	public  DefinitionAsset? Asset => _asset;
	
	public NodeIOAssetDefinition(ProjectNode owner) : base(owner)
	{
	}

	public override void Update()
	{
		_asset = DefinitionAsset.CreateFromFile(Owner.AbsolutePath, StudioGlobals.MigrationManager);
	}

	public override DefinitionAsset? Load()
	{
		return DefinitionAsset.CreateFromFile(Owner.AbsolutePath, StudioGlobals.MigrationManager);
	}

	public override void Save(DefinitionAsset data)
	{
		data.WriteToFile(Owner.AbsolutePath);
	}

	public override void ClearCache()
	{
		_asset = null;
	}

	public override void Import(string resourcePath)
	{
		DefinitionAsset? data = Load();
		if (data == null)
		{
			return;
		}

		data.ImportToResources(resourcePath);
		ClearCache();
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOAssetDefinitionSettings();
	}
}