using Mine.Framework;

namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetMaterial)]
public sealed class NodeIOMaterial : NodeIO<Material>
{
	public override string ReferenceType => nameof(AssetReference); // TODO - remove ?
	
	public NodeIOMaterial(ProjectNode owner) : base(owner)
	{
	}

	public override void UpdateCache()
	{
	}

	public override void ClearCache()
	{
	}

	public override string? Import(string resourcesRootPath)
	{
		return null;
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOMaterialSettings();
	}

	public override bool UpdateImportSettings(NodeIOSettings settings)
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