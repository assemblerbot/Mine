using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public abstract class NodeIO
{
	protected readonly ProjectNode Owner;

	protected NodeIO(ProjectNode owner)
	{
		Owner = owner;
	}

	public abstract void Load();
	public abstract void Save();
	public abstract void Import(string resourcePath);
	public abstract void ClearCache();
}