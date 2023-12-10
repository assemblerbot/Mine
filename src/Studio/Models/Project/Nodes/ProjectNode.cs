using Migration;
using RedHerring.Studio.UserInterface.Attributes;

namespace RedHerring.Studio.Models.Project.FileSystem;

public abstract class ProjectNode
{
	public          string    Name { get; }
	public readonly string    Path;
	public readonly string    RelativePath; // relative path inside Assets directory
	[ReadOnlyInInspector] public          bool      HasMetaFile;
	public          Metadata? Meta;

	public string Extension => System.IO.Path.GetExtension(Path).ToLower();	// cache if needed

	protected ProjectNode(string name, string path, string relativePath, bool hasMetaFile)
	{
		Name         = name;
		Path         = path;
		RelativePath = relativePath;
		HasMetaFile  = hasMetaFile;
	}

	public abstract void InitMeta(MigrationManager migrationManager, CancellationToken cancellationToken);

	public void UpdateMetaFile()
	{
		string metaPath = $"{Path}.meta";
		byte[] json     = MigrationSerializer.SerializeAsync(Meta, SerializedDataFormat.JSON, StudioModel.Assembly).GetAwaiter().GetResult();
		File.WriteAllBytes(metaPath, json);
	}

	protected void CreateMetaFile(MigrationManager migrationManager, string? hash)
	{
		string metaPath = $"{Path}.meta";
		
		// read if possible
		Metadata? meta = null;
		if (File.Exists(metaPath))
		{
			byte[] json = File.ReadAllBytes(metaPath);
			meta = MigrationSerializer.DeserializeAsync<Metadata, IMetadataMigratable>(null, json, SerializedDataFormat.JSON, migrationManager, true, StudioModel.Assembly).GetAwaiter().GetResult();
		}
		
		// write if needed
		if(meta == null || meta.Hash != hash)
		{
			meta ??= new Metadata();
			meta.UpdateGuid();
			meta.SetHash(hash);

			byte[] json = MigrationSerializer.SerializeAsync(meta, SerializedDataFormat.JSON, StudioModel.Assembly).GetAwaiter().GetResult();
			File.WriteAllBytes(metaPath, json);
		}

		Meta = meta;
	}

	public abstract void TraverseRecursive(Action<ProjectNode> process, TraverseFlags flags, CancellationToken cancellationToken);
}