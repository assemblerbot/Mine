using Mine.Studio;

namespace RedHerring.Studio.Models.Project.FileSystem;

public class ProjectScriptFileNode : ProjectNode
{
	public override string RelativeDirectoryPath => RelativePath.Substring(0, RelativePath.Length - Name.Length);
	public override bool   Exists                => File.Exists(AbsolutePath);

	public ProjectScriptFileNode(string name, string absolutePath, string relativePath) : base(name, absolutePath, relativePath, false)
	{
	}

	public override void Init(CancellationToken cancellationToken)
	{
		string guid;
		
		// try to parse file header
		ProjectScriptFileHeader? header = ProjectScriptFileHeader.CreateFromFile(AbsolutePath);
		if(header != null)
		{
			guid = header.Guid;
			SetNodeType(ProjectNodeTypeExtensions.FromScriptType(header.Type));
		}
		else
		{
			guid = RelativePath;
			SetNodeType(ProjectNodeType.ScriptFile);
		}

		IO = StudioGlobals.NodeIORegistry.CreateNodeIO(this);
		
		Meta = new Metadata
		       {
			       Guid = guid,
			       Hash = null,
		       };
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