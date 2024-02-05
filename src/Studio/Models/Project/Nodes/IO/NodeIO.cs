using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;

namespace Mine.Studio;

public abstract class NodeIO
{
	protected readonly ProjectNode Owner;

	protected NodeIO(ProjectNode owner)
	{
		Owner = owner;
	}

	public abstract void UpdateCache();
	public abstract void ClearCache();
	
	public abstract string? Import(string resourcesRootPath); // import file to Resources, returns relative path to resource or null

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