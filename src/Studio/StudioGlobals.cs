using System.Reflection;
using Migration;
using RedHerring.Studio.Models;

namespace Mine.Studio;

// singleton, just for globals that cannot change in runtime!
public sealed class StudioGlobals
{
	private static StudioGlobals _this;

	public static Assembly Assembly => typeof(StudioModel).Assembly; 
	
	private readonly MigrationManager _migrationManager = new(Assembly);
	public static    MigrationManager MigrationManager => _this._migrationManager;

	private readonly NodeIORegistry _nodeIORegistry = new();
	public static    NodeIORegistry NodeIORegistry => _this._nodeIORegistry;

	public StudioGlobals()
	{
		_this = this;
	}
}