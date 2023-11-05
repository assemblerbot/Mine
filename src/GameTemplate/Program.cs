using GameToolkit.Framework;

namespace GameToolkit.Game;

internal static class Program
{
	private static Engine _engine = null!;
	
	private static void Main(string[] args)
	{
		_engine = new Engine("GameTemplate");
		_engine.Run();
	}
}