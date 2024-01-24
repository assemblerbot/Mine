using Mine.Studio;

namespace RedHerring.Studio.Models.Project.FileSystem;

public sealed class ProjectAssetFileNode : ProjectNode
{
	public override string RelativeDirectoryPath => RelativePath.Substring(0, RelativePath.Length - Name.Length);
	public override bool   Exists                => File.Exists(AbsolutePath);
	
	public ProjectAssetFileNode(string name, string absolutePath, string relativePath) : base(name, absolutePath, relativePath, true)
	{
	}

	public override void Init(CancellationToken cancellationToken)
	{
		SetNodeType(ProjectNodeTypeExtensions.FromAssetExtension(Extension));
		IO = StudioGlobals.NodeIORegistry.CreateNodeIO(this);
		
		CreateMetaFile();
		if (Meta == null)
		{
			return;
		}

		if (Meta.NodeIOSettings == null)
		{
			Meta.NodeIOSettings = IO.CreateImportSettings();
			UpdateMetaFile();
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