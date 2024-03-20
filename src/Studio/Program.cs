using System.Numerics;
using Mine.Framework;

namespace Mine.Studio;

internal static class Program
{
	private static StudioGlobals _globals = null!;
	private static Engine        _engine  = null!;
	
	private static void Main(string[] args)
	{
		_engine  = new Engine(args, StudioComponent.Title, OnLoad, OnExit);
		_globals = new();

		_engine.Run();
	}

	private static void OnLoad()
	{
		Engine.World.Root.AddChild(new Entity("Studio").AddComponent<StudioComponent>().Entity);
	}

	private static void OnExit()
	{
	}
}