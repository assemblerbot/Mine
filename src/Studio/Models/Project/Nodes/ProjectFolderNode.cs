using System.Collections.ObjectModel;
using Migration;

namespace RedHerring.Studio.Models.Project.FileSystem;

public class ProjectFolderNode : ProjectNode
{
	public ObservableCollection<ProjectNode> Children { get; init; } = new();
	
	public ProjectFolderNode(string name, string path, string relativePath, bool hasMetaFile, ProjectNodeType type) : base(name, path, relativePath, hasMetaFile)
	{
		Type = type;
	}

	public override void InitMeta(MigrationManager migrationManager, CancellationToken cancellationToken)
	{
		if (HasMetaFile)
		{
			CreateMetaFile(migrationManager, null);
		}
		else
		{
			Meta = new Metadata
			       {
				       Guid = RelativePath,
				       Hash = ""
			       };
		}
	}

	public override void TraverseRecursive(Action<ProjectNode> process, TraverseFlags flags, CancellationToken cancellationToken)
	{
		if ((flags & TraverseFlags.Directories) != 0)
		{
			process(this);
		}

		foreach (ProjectNode child in Children)
		{
			if(cancellationToken.IsCancellationRequested)
			{
				return;
			}
			
			child.TraverseRecursive(process, flags, cancellationToken);
		}
	}
}