using GameToolkit.Framework;

namespace GameToolkit.Studio;

internal static class Program
{
	private static Engine _engine = null!;
	
	private static void Main(string[] args)
	{
		_engine = new Engine("GameToolkit - Studio", OnLoad, OnExit);
		_engine.Run();
	}

	private static void OnLoad()
	{
		Engine.Scene.Add(new GameObject("Studio").AddComponent<StudioComponent>());
	}

	private static void OnExit()
	{
	}
}