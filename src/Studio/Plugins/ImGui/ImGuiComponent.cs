using ImGuiNET;
using Mine.Framework;
using Silk.NET.Maths;
using Veldrid;

namespace Mine.ImGuiPlugin;

// built-in game object component
public sealed class ImGuiComponent : Component, IUpdatable, IRenderable
{
	public const string AssetsFolder    = "Plugins/ImGuiAssets/";
	public const string DefaultFontFile = AssetsFolder + "Roboto-Regular.ttf";
	public const string FontFARegular   = AssetsFolder + "fa-regular-400.ttf";
	public const string FontFASolid     = AssetsFolder + "fa-solid-900.ttf";
	public const int    Size            = 16;

	public int UpdateOrder = Int32.MinValue;
	public int GetUpdateOrder() => UpdateOrder;
	
	public int RenderOrder = Int32.MaxValue;
	public int GetRenderOrder() => RenderOrder;
	
	private ImGuiRenderer _renderer;
	private CommandList   _commandList;
	
	private ImGuiInputSnapshot _imGuiInputSnapshot;
	
	public ImGuiComponent()
	{
		Vector2D<int> windowSize = Engine.Window.Size;
		
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

		LoadDefaultFont();
		LoadIconsFont(FontFARegular);
		LoadIconsFont(FontFASolid);
		
		ImGui.GetIO().Fonts.Build();
		
		_renderer.RecreateFontDeviceTexture();
		
	}

	private unsafe void LoadDefaultFont()
	{
		try
		{
			byte[]? fontData = Engine.Resources.ReadResource(DefaultFontFile);
			ImFontConfig config = new()
			                      {
				                      MergeMode          = 0,
				                      OversampleH        = 1,
				                      OversampleV        = 1,
				                      PixelSnapH         = 1,
				                      RasterizerMultiply = 1,
				                      GlyphMinAdvanceX   = 1,
				                      GlyphMaxAdvanceX   = 256,
			                      };

			fixed (byte* ptr = fontData)
			{
				ImFontPtr font = ImGui.GetIO().Fonts.AddFontFromMemoryTTF((IntPtr) ptr, fontData.Length, Size, &config);
			}
		}
		catch(Exception e)
		{
			Console.WriteLine(e);
			ImGui.GetIO().Fonts.AddFontDefault();
		}
	}

	private unsafe void LoadIconsFont(string path)
	{
		byte[]? fontData = Engine.Resources.ReadResource(path);
		if (fontData != null)
		{
			ushort[] range = { FontAwesome6.IconMin, FontAwesome6.IconMax, 0 };

			fixed(byte* fontDataPtr = fontData)
			fixed(ushort* glyphRangePtr = range)
			{
				ImFontConfig config = new()
				                      {
					                      GlyphRanges        = glyphRangePtr,
					                      MergeMode          = 1,
					                      OversampleH        = 1,
					                      OversampleV        = 1,
					                      PixelSnapH         = 1,
					                      RasterizerMultiply = 1,
					                      GlyphMinAdvanceX   = 1,
					                      GlyphMaxAdvanceX   = 256,
				                      };
				
				ImFontPtr font = ImGui.GetIO().Fonts.AddFontFromMemoryTTF((IntPtr)fontDataPtr, fontData.Length, Size, &config);
			}
		}
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