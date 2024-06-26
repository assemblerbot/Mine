using System.Reflection;
using Migration;

namespace Mine.Studio;

// singleton, just for globals that cannot change in runtime!
public sealed class StudioGlobals
{
	private static StudioGlobals _this;

	public static Assembly Assembly => typeof(StudioModel).Assembly; 
	
	private readonly MigrationManager _migrationManager = new(Assembly);
	public static    MigrationManager MigrationManager => _this._migrationManager;

	private readonly ImporterRegistry _importerRegistry = new();
	public static    ImporterRegistry ImporterRegistry => _this._importerRegistry;

	public StudioGlobals()
	{
		_this = this;
	}
}