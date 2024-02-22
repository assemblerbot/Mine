namespace Mine.Studio;

public abstract class NodeIO
{
	protected readonly ProjectNode Owner;
	public abstract    string      ReferenceType { get; }

	protected NodeIO(ProjectNode owner)
	{
		Owner = owner;
	}

	public abstract void UpdateCache();
	public abstract void ClearCache();

	public abstract void Import(string resourcesRootPath, out string? relativeResourcePath);

	public abstract NodeIOSettings CreateImportSettings();
	public abstract bool           UpdateImportSettings(NodeIOSettings settings); // returns true if settings were changed
}

public abstract class NodeIO<TData> : NodeIO
{
	protected NodeIO(ProjectNode owner) : base(owner)
	{
	}

	public abstract TData? Load(); 
	public abstract void   Save(TData                          data);
}