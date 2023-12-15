using Migration;

namespace RedHerring.Studio.Models.Project.FileSystem;

public class ProjectScriptFileNode : ProjectNode
{
	public override string RelativeDirectoryPath => RelativePath.Substring(0, RelativePath.Length - Name.Length);
	
	[Serializable]
	private class FileId
	{
		public string Guid { get; set; }
		public string Type { get; set; } // TODO - constants or something
	}

	public ProjectScriptFileNode(string name, string path, string relativePath) : base(name, path, relativePath, false)
	{
		SetNodeType(ProjectNodeType.ScriptFile);
	}

	public override void InitMeta(MigrationManager migrationManager, CancellationToken cancellationToken)
	{
		string guid = RelativePath;
		
		// try to parse file header
		ProjectScriptFileHeader.FileId? fileId = ProjectScriptFileHeader.ReadFromFile(Path);
		if(fileId != null)
		{
			guid = fileId.Guid;
			SetNodeType(ProjectNodeType.ScriptDefinitionTemplate);
		}

		Meta = new Metadata
		       {
			       Guid = guid,
			       Hash = "",
		       };
	}

	public override void TraverseRecursive(Action<ProjectNode> process, TraverseFlags flags, CancellationToken cancellationToken)
	{
		if ((flags & TraverseFlags.Files) != 0)
		{
			process(this);
		}
	}
}