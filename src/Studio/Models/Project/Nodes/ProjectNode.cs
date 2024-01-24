using Migration;
using Mine.Studio;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models.Project.FileSystem;

public abstract class ProjectNode
{
	[ReadOnlyInInspector] public ProjectNodeType Type = ProjectNodeType.Uninitialized;

	public          string Name { get; }
	public readonly string AbsolutePath;
	public readonly string RelativePath; // relative path inside Assets directory
	public abstract string RelativeDirectoryPath { get; }

	[ReadOnlyInInspector] public bool HasMetaFile;
	
	public Metadata? Meta;
	public NodeIO?   IO;

	public          string Extension => Path.GetExtension(AbsolutePath).ToLower(); // cache if needed
	public abstract bool   Exists    { get; }

	protected ProjectNode(string name, string absolutePath, string relativePath, bool hasMetaFile)
	{
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

	public void UpdateMetaFile()
	{
		string metaPath = $"{AbsolutePath}.meta";
		byte[] json     = MigrationSerializer.SerializeAsync(Meta, SerializedDataFormat.JSON, StudioGlobals.Assembly).GetAwaiter().GetResult();
		File.WriteAllBytes(metaPath, json);
	}

	public void SetNodeType(ProjectNodeType type)
	{
		Type = type;
	}

	public T? GetNodeIO<T>() where T : NodeIO
	{
		// TODO do in separate thread + add some management (unload?)
		// _content ??= studioModel.Project.ContentRegistry.LoadContent(studioModel, this);
		// return _content as T;
		return null;
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