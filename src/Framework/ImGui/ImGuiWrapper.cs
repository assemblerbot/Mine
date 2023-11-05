using ImGuiNET;
using Silk.NET.Maths;
using Veldrid;

namespace GameToolkit.Framework;

public sealed class ImGuiWrapper : IDisposable
{
	public const string DefaultPath     = "Resources";
	public const string DefaultFontFile = "Roboto-Regular.ttf";
	public const int    Size            = 16;
	
	private ImGuiRenderer _renderer;
	private CommandList _commandList;
	
	private ImGuiInputSnapshot _imGuiInputSnapshot;
	//private ImFontPtr _defaultFont = null!;

	public ImGuiWrapper(Vector2D<int> windowSize)
	{
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

	private void LoadFonts()
	{
		ImGui.GetIO().Fonts.Clear();
		
		ImFontPtr font = ImGui.GetIO().Fonts.AddFontFromFileTTF(Path.Combine(DefaultPath, DefaultFontFile), Size);
		if (!font.IsLoaded())
		{
			ImGui.GetIO().Fonts.AddFontDefault();
		}
		
		_renderer.RecreateFontDeviceTexture();
	}

	public void Dispose()
	{
		_renderer.ClearCachedImageResources();
		_renderer.DestroyDeviceObjects();
		_renderer.Dispose();

		_commandList.Dispose();
	}

	public void Update(float timeDelta)
	{
		_imGuiInputSnapshot.Update();
		_renderer?.Update(timeDelta, _imGuiInputSnapshot);
	}

	public void Render()
	{
		_commandList.Begin();
		_commandList.SetFramebuffer(Engine.Renderer.Device.SwapchainFramebuffer);
		_renderer?.Render(Engine.Renderer.Device, _commandList);
		_commandList.End();
		Engine.Renderer.Device.SubmitCommands(_commandList);
	}
	
	public void Resize(Vector2D<int> size)
	{
		_renderer?.WindowResized(size.X, size.Y);
	}
}