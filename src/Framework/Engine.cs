using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Veldrid;

namespace Mine.Framework;

public sealed class Engine
{
	private static Engine _instance = null!;
	public static  Engine Instance => _instance;
	
	private readonly GraphicsBackend _graphicsBackend;
	private readonly Types           _types  = new();
	private readonly Config          _config = new();
	private readonly EngineWindow    _window;
	private          Graphics        _graphics  = null!;
	private          Input           _input     = null!;
	private          World           _world     = new();
	private readonly Resources       _resources = new();
	private          Shared          _shared    = new();
	private readonly Paths           _paths;
	private readonly Timing          _timing = new();

	public static Types     Types     => _instance._types;
	public static Config    Config    => _instance._config;
	public static IWindow   Window    => _instance._window.NativeWindow;
	public static Graphics  Graphics  => _instance._graphics;
	public static Input     Input     => _instance._input;
	public static World     World     => _instance._world;
	public static Resources Resources => _instance._resources;
	public static Shared    Shared    => _instance._shared;
	public static Paths     Paths     => _instance._paths;
	public static Timing    Timing    => _instance._timing;
	
	private Action? _onLoad;
	private Action? _onExit;

	private bool _exitRequested = false;

	public         Action<bool>? OnFocusChanged;
	private        bool          _hasFocus = true;
	public  static bool          HasFocus => _instance._hasFocus;

	private static ulong _uniqueId = 0;
	public static  ulong NextUniqueId => ++_uniqueId; // engine-wide unique id that can be used for anything in runtime, not in save/load state! Never returns 0.
	
	public Engine(string[] applicationArguments, string applicationName, Action? onLoad = null, Action? onExit = null)
	{
		_instance = this;
		
		_paths = new Paths(applicationName);

		ParseArguments(applicationArguments);
		
		_onLoad = onLoad;
		_onExit = onExit;
		
		_graphicsBackend = GetPreferredBackend();
		
		_window = new EngineWindow(applicationName, _graphicsBackend);

		_window.OnResizeCallback  += OnResize;
		_window.OnLoadCallback    += OnLoad;
		_window.OnUpdateCallback  += OnUpdate;
		_window.OnRenderCallback  += OnRender;
		_window.OnCloseCallback   += OnExit;
		_window.View.FocusChanged += OnFocusChangedHandler;

		_config.Load();
	}

	public void Run()
	{
		_window.Run();
	}

	public static void Exit()
	{
		_instance._exitRequested = true;
	}
	
	private void OnResize(Vector2D<int> size)
	{
		Vector2Int newSize = new(size.X, size.Y);
		_graphics.Resize(newSize);
		_world.WindowSizeChanged(newSize);
	}
	
	private void OnLoad()
	{
		_graphics = new Graphics(_window.View, _graphicsBackend);
		_input    = new Input(_window.View);
		_shared.Init();
		_resources.Init();
		_onLoad?.Invoke();
	}

	private void OnUpdate(double timeDelta)
	{
		_timing.UpdateCalled();
		_world.Update(timeDelta);
		_input.EndOfFrame();

		if (_exitRequested)
		{
			_instance._window.Close();
		}
	}

	private void OnRender(double timeDelta)
	{
		_timing.RenderCalled();
		_graphics.BeginOfFrame();
		_world.Render();
		_graphics.EndOfFrame();
	}

	private void OnExit()
	{
		_window.View.FocusChanged -= OnFocusChangedHandler;

		_config?.Save();
		_onExit?.Invoke();

		_world?.Dispose();
		_world = null!;

		_input?.Dispose();
		_input = null!;

		_shared?.Dispose();
		_shared = null!;
		
		_graphics?.Dispose();
		_graphics = null!;
	}

	private void OnFocusChangedHandler(bool hasFocus)
	{
		_hasFocus = hasFocus;
		OnFocusChanged?.Invoke(hasFocus);
	}
	
	private void ParseArguments(string[] arguments)
	{
		for (int i = 0; i < arguments.Length; ++i)
		{
			if (arguments[i] == "--resources")
			{
				Resources.RootRelativePath = arguments[++i];
				continue;
			}
			
			
		}
	}

	// TODO - shouldn't be here
	private GraphicsBackend GetPreferredBackend()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan)
				? GraphicsBackend.Vulkan
				: GraphicsBackend.Direct3D11;
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return GraphicsDevice.IsBackendSupported(GraphicsBackend.Metal)
				? GraphicsBackend.Metal
				: GraphicsBackend.OpenGL;
		}

		return GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan)
			? GraphicsBackend.Vulkan
			: GraphicsBackend.OpenGL;
	}
}