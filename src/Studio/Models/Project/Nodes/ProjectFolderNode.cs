namespace Mine.Studio;

public class ProjectFolderNode : ProjectNode
{
	public override string RelativeDirectoryPath => RelativePath;
	public override bool   Exists                => Directory.Exists(AbsolutePath);

	public List<ProjectNode> Children { get; } = new();
	
	public ProjectFolderNode(ProjectModel project, string name, string absolutePath, string relativePath, bool hasMetaFile, ProjectNodeKind kind)
		: base(project, name, absolutePath, relativePath, hasMetaFile)
	{
		SetNodeType(kind); // set here, because we already know that it's a folder but we don't know if it's an asset folder or script folder, that comes from parameter
	}

	public override void Init(CancellationToken cancellationToken)
	{
		Importer = StudioGlobals.ImporterRegistry.CreateImporter(this);
		Importer.UpdateCache();
		
		if (HasMetaFile)
		{
			CreateMetaFile();
		}
		else
		{
			Meta = new Metadata
			       {
				       Guid = RelativePath,
				       Hash = null
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

	public override ProjectNode? FindNode(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return this;
		}

		int index = path.IndexOfAny(new [] {'\\','/'});
		if (index == -1)
		{
			// end of path
			return Children.FirstOrDefault(node => node.Name == path);
		}

		string       folderName = path.Substring(0, index);
		ProjectNode? node       = Children.FirstOrDefault(node => node.Name == folderName);

		if (node == null)
		{
			// not found
			return null;
		}

		// continue with shorter path
		return node.FindNode(path.Substring(folderName.Length + 1));
	}

	public ProjectNode? FindChild(string name)
	{
		return Children.FirstOrDefault(child => child.Name == name);
	}
}