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

	public abstract void Update();     // update only most important things that needs to be always up-to-date
	public abstract void Load();       // load and parse whole file
	public abstract void Save();       // save file to assets
	public abstract void ClearCache(); // clear loaded file
	
	public abstract void           Import(string resourcePath); // import file to Resources
	public abstract NodeIOSettings CreateImportSettings();      // create import settings instance
}