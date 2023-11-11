using ImGuiNET;
using Silk.NET.Maths;
using Veldrid;

namespace GameToolkit.Framework;

// built-in game object component
public sealed class ImGuiComponent : Component, IUpdatable, IRenderable
{
	public const string DefaultFontFile = "Roboto-Regular.ttf";
	public const int    Size            = 16;

	public int UpdateOrder = Int32.MinValue;
	public int GetUpdateOrder() => UpdateOrder;
	
	public int RenderOrder = Int32.MaxValue;
	public int GetRenderOrder() => RenderOrder;
	
	private ImGuiRenderer _renderer;
	private CommandList _commandList;
	
	private ImGuiInputSnapshot _imGuiInputSnapshot;
	//private ImFontPtr _defaultFont = null!;
	
	public ImGuiComponent()
	{
		Vector2D<int> windowSize = Engine.Window.View.Size;
		
		_imGuiInputSnapshot = new ImGuiInputSnapshot();
		
		_renderer = new ImGuiRenderer(
			Engine.Renderer.Device,
			Engine.Renderer.Device.MainSwapchain.Framebuffer.OutputDescription,
			windowSize.X,
			windowSize.Y
		);
		
		_commandList = Engine.Renderer.Factory.CreateCommandList();

		LoadFonts();
	}

	public override void AfterAddedToScene()
	{
		Scene.RegisterUpdatable(this);
		Scene.RegisterRenderable(this);
	}

	public override void BeforeRemovedFromScene()
	{
		Scene.UnregisterUpdatable(this);
		Scene.UnregisterRenderable(this);
	}

	private void LoadFonts()
	{
		ImGui.GetIO().Fonts.Clear();
		
		ImFontPtr font = ImGui.GetIO().Fonts.AddFontFromFileTTF(Path.Combine(Engine.ResourceDirectory, DefaultFontFile), Size);
		if (!font.IsLoaded())
		{
			ImGui.GetIO().Fonts.AddFontDefault();
		}
		
		_renderer.RecreateFontDeviceTexture();
	}

	public override void Dispose()
	{
		_renderer.ClearCachedImageResources();
		_renderer.DestroyDeviceObjects();
		_renderer.Dispose();

		_commandList.Dispose();
	}

	public void Update(double timeDelta)
	{
		_imGuiInputSnapshot.Update();
		_renderer?.Update((float)timeDelta, _imGuiInputSnapshot);
	}

	public void Render()
	{
		_commandList.Begin();
		_commandList.SetFramebuffer(Engine.Renderer.Device.SwapchainFramebuffer);
		_renderer?.Render(Engine.Renderer.Device, _commandList);
		_commandList.End();
		Engine.Renderer.Device.SubmitCommands(_commandList);
	}
	
	public void WindowResized(Vector2Int size)
	{
		_renderer?.WindowResized(size.x, size.y);
	}
}