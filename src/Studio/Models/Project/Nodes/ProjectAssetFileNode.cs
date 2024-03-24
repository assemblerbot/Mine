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
		Importer = StudioGlobals.ImporterRegistry.CreateImporter(this);
		Importer.UpdateCache();
		
		CreateMetaFile();
		if (Meta == null)
		{
			return;
		}

		bool ioSettingsChanged = false;
		if (Meta.ImporterSettings == null)
		{
			Meta.ImporterSettings = Importer.CreateImportSettings();
			ioSettingsChanged   = true;
		}

		ioSettingsChanged |= Importer.UpdateImportSettings(Meta.ImporterSettings);

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