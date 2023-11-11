using GameToolkit.Framework;

namespace GameToolkit.Studio;

internal static class Program
{
	private static Engine     _engine = null!;
	private static StudioMain _studio = null!;
	
	private static void Main(string[] args)
	{
		_engine = new Engine("GameToolkit - Studio", OnLoad, OnExit);
		_engine.Run();
	}

	private static void OnLoad()
	{
		_studio = new StudioMain();
		_studio.OnLoad();
	}

	private static void OnExit()
	{
		_studio.OnExit();
	}
}