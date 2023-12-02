using Mine.Framework;

namespace Template;

internal static class Program
{
	private const string  _name   = "Template";
	private static Engine _engine = null!;
	
	private static void Main(string[] args)
	{
		_engine = new Engine(args, _name, OnLoad, OnExit);
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