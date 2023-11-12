using Mine.Framework;

namespace Template;

internal static class Program
{
	private static Engine _engine = null!;
	
	private static void Main(string[] args)
	{
		_engine = new Engine(args, "Template", OnLoad, OnExit);
		_engine.Run();
	}

	private static void OnLoad()
	{
		Engine.Scene.Add(new GameObject("Game").AddComponent<GameComponent>());
	}

	private static void OnExit()
	{
	}
}