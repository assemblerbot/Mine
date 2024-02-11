namespace Mine.Studio;

public sealed class ProjectRootNode : ProjectFolderNode
{
	public ProjectRootNode(ProjectModel project, string name, string absolutePath, ProjectNodeType type)
		: base(project, name, absolutePath, "", false, type)
	{
	}

	public override void Init(CancellationToken cancellationToken)
	{
		Meta = new Metadata
		       {
			       Guid = Name,
			       Hash = $"#{Name}" // # is not valid base64 symbol, so this hash will be unique no matter what Name is
		       };
	}
}