using GameToolkit.Framework;
using GameToolkit.Studio.Components;

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
		GameObject testGameObject = new GameObject("Test Object").AddComponent(new TestRenderComponent());
		Engine.Scene.Add(testGameObject);
		testGameObject.CallOnComponentsInHierarchy<TestRenderComponent>(x => x.Init());
	}

	private static void OnExit()
	{
		
	}
}