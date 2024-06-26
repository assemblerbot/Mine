using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Extensions.Veldrid;
using Veldrid;

namespace Mine.Framework;

public class EngineWindow
{
	private readonly IWindow _window;
	public           IWindow NativeWindow => _window;
	public           IView   View         => _window;

	public Action?                OnLoadCallback;
	public Action<Vector2D<int>>? OnResizeCallback;
	public Action<double>?        OnUpdateCallback;
	public Action<double>?        OnRenderCallback;
	public Action?                OnCloseCallback;

	public EngineWindow(string title, GraphicsBackend graphicsBackend)
	{
		WindowOptions opts = new()
		                     {
			                     Title                   = title,
			                     Position                = new Vector2D<int>(100, 100),
			                     Size                    = new Vector2D<int>(960, 540),
			                     API                     = graphicsBackend.ToGraphicsAPI(),
			                     VSync                   = true,
			                     ShouldSwapAutomatically = false,
		                     };
        
		_window = Window.Create(opts);
        
		_window.Load    += OnLoad;
		_window.Update  += OnUpdate;
		_window.Render  += OnRender;
		_window.Closing += OnClose;
		_window.Resize  += OnResize;
	}

	public void Run()
	{
		_window.Run();
	}

	public void Close()
	{
		_window?.Close();
	}
	
	private void OnLoad()
	{
		_window!.IsVisible = true;
		OnLoadCallback?.Invoke();
	}

	private void OnResize(Vector2D<int> size)
	{
		OnResizeCallback?.Invoke(size);
	}

	private void OnUpdate(double timeDelta)
	{
		OnUpdateCallback?.Invoke(timeDelta);
	}

	private void OnRender(double timeDelta)
	{
		OnRenderCallback?.Invoke(timeDelta);
	}

	private void OnClose()
	{
		OnCloseCallback?.Invoke();
	}
}