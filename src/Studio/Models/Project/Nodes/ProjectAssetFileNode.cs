using Migration;
using Mine.Studio;
using RedHerring.Studio.Models.Project.Importers;

namespace RedHerring.Studio.Models.Project.FileSystem;

public sealed class ProjectAssetFileNode : ProjectNode
{
	public override string RelativeDirectoryPath => RelativePath.Substring(0, RelativePath.Length - Name.Length);
	public override bool   Exists                => File.Exists(AbsolutePath);
	
	public ProjectAssetFileNode(string name, string absolutePath, string relativePath) : base(name, absolutePath, relativePath, true)
	{
	}

	public override void Init(MigrationManager migrationManager, ImporterRegistry importerRegistry, NodeIORegistry nodeIORegistry, CancellationToken cancellationToken)
	{
		SetNodeType(ProjectNodeTypeExtensions.FromAssetExtension(Extension));
		IO = nodeIORegistry.CreateNodeIO(this);
		
		CreateMetaFile(migrationManager);
		if (Meta == null)
		{
			return;
		}

		if (Meta.ImportSettings == null)
		{
			Importer importer = importerRegistry.GetImporter(Extension);
			Meta.ImportSettings = importer.CreateSettings();
		}
	}

	public override void TraverseRecursive(Action<ProjectNode> process, TraverseFlags flags, CancellationToken cancellationToken)
	{
		if ((flags & TraverseFlags.Files) != 0)
		{
			process(this);
		}
	}

	public override ProjectNode? FindNode(string path)
	{
		return null;
	}
}