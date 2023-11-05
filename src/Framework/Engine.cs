using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Veldrid;

namespace GameToolkit.Framework;

public sealed class Engine
{
	private static Engine _instance = null!;
	public static  Engine Instance => _instance;
	
	private readonly GraphicsBackend _graphicsBackend;
	private readonly EngineWindow    _window;
	private          Renderer        _renderer     = null!;
	private          Input           _input        = null!;
	private          Scene           _scene        = new();
	private readonly ImGuiWrapper    _imGuiWrapper = new();

	public static Renderer Renderer => _instance._renderer;
	public static Input    Input    => _instance._input;
	public static Scene    Scene    => _instance._scene;

	private Action? _onLoad;
	private Action? _onExit;

	public Engine(string windowTitle, Action? onLoad = null, Action? onExit = null)
	{
		_instance = this;
		
		_onLoad = onLoad;
		_onExit = onExit;
		
		_graphicsBackend = GetPreferredBackend();
		
		_window = new EngineWindow(windowTitle, _graphicsBackend);

		_window.OnResizeCallback += OnResize;
		_window.OnLoadCallback   += OnLoad;
		_window.OnUpdateCallback += OnUpdate;
		_window.OnDrawCallback   += OnDraw;
		_window.OnCloseCallback  += OnExit;
	}

	public void Run()
	{
		_window.Run();
	}

	private void OnResize(Vector2D<int> size)
	{
		_renderer.OnResize(size);
	}
	
	private void OnLoad()
	{
		_renderer = new Renderer(_window.View, _graphicsBackend);
		_input    = new Input(_window.View);
		_onLoad?.Invoke();
	}

	private void OnUpdate(double timeDelta)
	{
		_scene.Update(timeDelta);
	}

	private void OnDraw(double timeDelta)
	{
		_renderer.Draw();
	}

	private void OnExit()
	{
		_onExit?.Invoke();

		_scene?.Dispose();
		_scene = null!;

		_input?.Dispose();
		_input = null!;
		
		_renderer?.Dispose();
		_renderer = null!;
	}
	
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