using Mine.Framework;

namespace Mine.Studio;

internal static class Program
{
	private static StudioGlobals _globals = null!;
	private static Engine        _engine  = null!;
	
	private static void Main(string[] args)
	{
		_engine  = new Engine(args, "MINE Studio", OnLoad, OnExit);
		_globals = new();

		_engine.Run();
	}

	private static void OnLoad()
	{
		Engine.Scene.Add(new GameObject("Studio").AddComponent<StudioComponent>().GameObject);
	}

	private static void OnExit()
	{
	}
}