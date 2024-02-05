using Mine.Framework;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;

namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetMaterial)]
public sealed class NodeIOMaterial : NodeIO<Material>
{
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