using Migration;
using Mine.Studio;

namespace Mine.Studio;

public abstract class ProjectNode : ISelectable
{
	// TODO - type of node should be System.Type - not enum and it should contain the type of referenced class
	
	[HideInInspector]     public readonly ProjectModel    Project;
	[ReadOnlyInInspector] public          ProjectNodeType Type        = ProjectNodeType.Uninitialized;
	[HideInInspector]     public          Type?           ContentType = null;

	public          string Name { get; }
	public readonly string AbsolutePath;
	public readonly string RelativePath; // relative path inside Assets directory
	public abstract string RelativeDirectoryPath { get; }

	[ReadOnlyInInspector] public bool HasMetaFile;
	
	public    Metadata? Meta;
	protected NodeIO?   IO;

	public          string Extension => Path.GetExtension(AbsolutePath).ToLower(); // cache if needed
	public abstract bool   Exists    { get; }

	protected ProjectNode(ProjectModel project, string name, string absolutePath, string relativePath, bool hasMetaFile)
	{
		Project      = project;
		Name         = name;
		AbsolutePath = absolutePath;
		RelativePath = relativePath;
		HasMetaFile  = hasMetaFile;
	}

	public abstract void Init(CancellationToken cancellationToken);

	public void ResetMetaHash()
	{
		Meta?.SetHash(null);
	}

	public void ApplyChanges()
	{
		UpdateMetaFile();
	}

	public void UpdateMetaFile()
	{
		string metaPath = $"{AbsolutePath}.meta";
		byte[] json     = MigrationSerializer.Serialize(Meta, SerializedDataFormat.JSON, StudioGlobals.Assembly);
		File.WriteAllBytes(metaPath, json);
	}

	public void SetNodeType(ProjectNodeType type)
	{
		Type = type;
	}

	public T? GetNodeIO<T>() where T : NodeIO
	{
		return IO as T;
	}

	protected void CreateMetaFile()
	{
		string metaPath = $"{AbsolutePath}.meta";
		
		// read if possible
		Metadata? meta = null;
		if (File.Exists(metaPath))
		{
			byte[] json = File.ReadAllBytes(metaPath);
			meta = MigrationSerializer.DeserializeAsync<Metadata, IMetadataMigratable>(null, json, SerializedDataFormat.JSON, StudioGlobals.MigrationManager, true, StudioGlobals.Assembly).GetAwaiter().GetResult();
		}
		
		// write if needed
		if(meta == null)
		{
			meta ??= new Metadata();
			meta.UpdateGuid();

			byte[] json = MigrationSerializer.SerializeAsync(meta, SerializedDataFormat.JSON, StudioGlobals.Assembly).GetAwaiter().GetResult();
			File.WriteAllBytes(metaPath, json);
		}

		Meta = meta;
	}

	public abstract void TraverseRecursive(Action<ProjectNode> process, TraverseFlags flags, CancellationToken cancellationToken);

	public abstract ProjectNode? FindNode(string path);

	public override string ToString()
	{
		return Name;
	}
}