using Mine.Studio;

namespace Mine.Studio;

public sealed class ProjectAssetFileNode : ProjectNode
{
	public override string RelativeDirectoryPath => RelativePath.Substring(0, RelativePath.Length - Name.Length);
	public override bool   Exists                => File.Exists(AbsolutePath);
	
	public ProjectAssetFileNode(ProjectModel project, string name, string absolutePath, string relativePath)
		: base(project, name, absolutePath, relativePath, true)
	{
	}

	public override void Init(CancellationToken cancellationToken)
	{
		SetNodeType(ProjectNodeTypeExtensions.FromAssetExtension(Extension));
		IO = StudioGlobals.NodeIORegistry.CreateNodeIO(this);
		IO.UpdateCache();
		
		CreateMetaFile();
		if (Meta == null)
		{
			return;
		}

		bool ioSettingsChanged = false;
		if (Meta.NodeIOSettings == null)
		{
			Meta.NodeIOSettings = IO.CreateImportSettings();
			ioSettingsChanged   = true;
		}

		ioSettingsChanged |= IO.UpdateImportSettings(Meta.NodeIOSettings);

		if (ioSettingsChanged)
		{
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