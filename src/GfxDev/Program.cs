﻿using Gfx;
using Silk.NET.Maths;
using Silk.NET.Windowing;

GfxTestApplication app = new ();
app.Run();

internal class GfxTestApplication
{
	private const GraphicsBackend _graphicsBackend = GraphicsBackend.Vulkan;
	private       IWindow         _window;
	public        IWindow         NativeWindow => _window;
	public        IView           View         => _window;

	private GfxApi? _api;
	
	public void Run()
	{
		InitWindow();
	}

	private void InitWindow()
	{
		//GraphicsBackend graphicsBackend = GfxApi.GetDefaultBackend();
		
		WindowOptions opts = new()
		                     {
			                     Title                   = "GfxTest",
			                     Position                = new Vector2D<int>(100, 100),
			                     Size                    = new Vector2D<int>(1024, 720),
			                     API                     = _graphicsBackend.ToGraphicsAPI(),
			                     VSync                   = true,
			                     ShouldSwapAutomatically = false,
			                     WindowState             = WindowState.Normal,
			                     WindowBorder            = WindowBorder.Resizable,
			                     IsVisible = true,
		                     };
        
		_window = Window.Create(opts);
		
		_window.Load    += OnLoad;
		_window.Update  += OnUpdate;
		_window.Render  += OnRender;
		_window.Closing += OnClose;
		_window.Resize  += OnResize;
	}

	private void InitGfx()
	{
		_api = GfxApi.Create(_graphicsBackend, _window);
		MainLoop();
		CleanUp();
	}

	private void MainLoop()
	{
		_window.Run();
	}

	private void CleanUp()
	{
		_api?.Dispose();
		_window.Dispose();
	}

	private void OnLoad()
	{
		InitGfx();
	}

	private void OnUpdate(double obj)
	{
	}

	private void OnRender(double obj)
	{
	}

	private void OnClose()
	{
	}

	private void OnResize(Vector2D<int> obj)
	{
	}
}