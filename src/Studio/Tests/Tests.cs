using System.Runtime.CompilerServices;
using Mine.Framework;

namespace Mine.Studio;

public static class Tests
{
	private static string _output = "";
	
	private sealed class ComponentA : Component
	{
		public override void AfterAddedToWorld()      => _output += "A+";
		public override void BeforeRemovedFromWorld() => _output += "A-";
		public override void Dispose()                => _output += "Ax";
	}

	private sealed class ComponentB : Component
	{
		public override void AfterAddedToWorld()      => _output += "B+";
		public override void BeforeRemovedFromWorld() => _output += "B-";
		public override void Dispose()                => _output += "Bx";
	}

	private sealed class ComponentC : Component
	{
		public override void AfterAddedToWorld()
		{
			_output += "C+";
			Entity b = new Entity("B").AddComponent<ComponentB>().Entity;
			Entity.AddChild(b);
		}

		public override void BeforeRemovedFromWorld()
		{
			_output += "C-";
		}

		public override void Dispose()                => _output += "Cx";
	}
	
	private static void CheckAndLog(string correct, [CallerLineNumber] int callerLineNumber = default)
	{
		if (_output == correct)
		{
			ConsoleViewModel.LogInfo($"Test called from line {callerLineNumber} PASSED");
		}
		else
		{
			ConsoleViewModel.LogError($"Test called from line {callerLineNumber} FAILED");
		}
	}
	
	public static void Run()
	{
		Test1();
		Test2();
		Test3();
		Test4();
		Test5();
	}

	// add remove
	private static void Test1()
	{
		_output = "";
		Entity a = new Entity().AddComponent<ComponentA>().Entity;
		Entity b = new Entity().AddComponent<ComponentB>().Entity;
		a.AddChild(b);
		Engine.World.Root.AddChild(a);
		Engine.World.Root.RemoveChild(a);
		CheckAndLog("A+B+B-A-");
	}

	// add destroy
	private static void Test2()
	{
		_output = "";
		Entity a = new Entity().AddComponent<ComponentA>().Entity;
		Entity b = new Entity().AddComponent<ComponentB>().Entity;
		a.AddChild(b);
		Engine.World.Root.AddChild(a);
		Engine.World.Root.DestroyChild(a);
		CheckAndLog("A+B+B-A-BxAx");
	}

	// reparent
	private static void Test3()
	{
		_output = "";
		Entity a = new Entity().AddComponent<ComponentA>().Entity;
		Entity b = new Entity().AddComponent<ComponentB>().Entity;
		a.AddChild(b);
		Engine.World.Root.AddChild(a);
		Engine.World.Root.AddChild(b);
		Engine.World.Root.RemoveChild(a);
		Engine.World.Root.RemoveChild(b);
		CheckAndLog("A+B+A-B-");
	}

	// add remove inside event functions
	private static void Test4()
	{
		_output = "";
		Entity a = new Entity().AddComponent<ComponentA>().Entity;
		Entity c = new Entity().AddComponent<ComponentC>().Entity;
		a.AddChild(c);
		Engine.World.Root.AddChild(a);
		Engine.World.Root.RemoveChild(a);
		CheckAndLog("A+C+B+B-C-A-");
	}
	// add remove inside event functions
	private static void Test5()
	{
		_output = "";
		Entity a = new Entity().AddComponent<ComponentA>().Entity;
		Entity b = new Entity().AddComponent<ComponentB>().Entity;
		Entity a2 = new Entity().AddComponent<ComponentA>().Entity;
		Entity b2 = new Entity().AddComponent<ComponentB>().Entity;
		Engine.World.Root.AddChild(a);
		a.AddChild(b);
		a.AddChild(a2);
		a.AddChild(b2);
		a.DestroyChildren();
		Engine.World.Root.DestroyChild(a);
		CheckAndLog("A+B+A+B+B-BxA-AxB-BxA-Ax");
	}
}