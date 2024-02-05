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

	public override void UpdateCache()
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

	public override string? Import(string resourcesRootPath)
	{
		DefinitionAsset? data = DefinitionAsset.CreateFromFile(Owner.AbsolutePath, StudioGlobals.MigrationManager);
		if (data == null)
		{
			return null;
		}

		string path = Path.Join(resourcesRootPath, Owner.RelativePath);
		data.ImportToResources(path);

		return Owner.RelativePath;
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOAssetDefinitionSettings();
	}

	public override bool UpdateImportSettings(NodeIOSettings settings)
	{
		return false;
	}
}