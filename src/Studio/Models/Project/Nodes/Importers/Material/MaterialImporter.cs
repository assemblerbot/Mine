using Mine.Framework;

namespace Mine.Studio;

[Importer(ProjectNodeType.AssetMaterial)]
public sealed class MaterialImporter : Importer<Material>
{
	public override string ReferenceType => nameof(AssetReference); // TODO - remove ?
	
	public MaterialImporter(ProjectNode owner) : base(owner)
	{
	}

	public override void UpdateCache()
	{
	}

	public override void ClearCache()
	{
	}

	public override void Import(string resourcesRootPath, out string? relativeResourcePath)
	{
		relativeResourcePath = null;
	}

	public override ImporterSettings CreateImportSettings()
	{
		return new MaterialImporterSettings();
	}

	public override bool UpdateImportSettings(ImporterSettings settings)
	{
		return false;
	}

	public override Material? Load()
	{
		throw new NotImplementedException();
	}

	public override void Save(Material data)
	{
		throw new NotImplementedException();
	}
}