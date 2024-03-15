using Veldrid;

namespace Mine.Framework;

public class RendererComponent : Component, IRenderer
{
	public int          RenderOrder { get; private set; }
	public ulong        RenderMask  { get; private set; }
	public Clipper      Clipper;
	public List<string> Passes = new();

	public bool            ClearColorTarget      = true;
	public Color4FloatRGBA ClearColor = Color4FloatRGBA.Black;

	public bool  ClearDepthTarget = true;
	public float ClearDepth       = 1f;

	public bool  ClearStencilTarget = false;
	public byte  ClearStencil     = 0;

	private CommandList? _commandList;
	private Framebuffer? _framebuffer;

	public RendererComponent(int renderOrder, ulong renderMask, Clipper clipper)
	{
		RenderOrder = renderOrder;
		RenderMask  = renderMask;
		Clipper     = clipper;
	}

	public override void AfterAddedToWorld()
	{
		_commandList = Engine.Graphics.Factory.CreateCommandList();
		
		Engine.World.RegisterRenderer(this);
	}

	public override void BeforeRemovedFromWorld()
	{
		Engine.World.UnregisterRenderer(this);

		_commandList?.Dispose();
		_commandList = null!;

		_framebuffer?.Dispose();
		_framebuffer = null;
	}

	public virtual void Render()
	{
		if (_commandList is null)
		{
			throw new NullReferenceException("Command buffer cannot be null!");
		}

		_commandList.Begin();

		// set frame buffer
		_commandList.SetFramebuffer(Engine.Graphics.Device.SwapchainFramebuffer); // TODO - texture

		// clear color target
		if (ClearColorTarget)
		{
			_commandList.ClearColorTarget(0, ClearColor.VeldridRgbaFloat); // TODO - target index is always 0
		}

		// clear depth-stencil target
		if (ClearDepthTarget)
		{
			if (ClearStencilTarget)
			{
				_commandList.ClearDepthStencil(ClearDepth, ClearStencil);
			}
			else
			{
				_commandList.ClearDepthStencil(ClearDepth);
			}
		}

		// render
		RenderMeshes();
		_commandList.End();
		Engine.Graphics.Device.SubmitCommands(_commandList);
	}
	
	public virtual void WindowResized(Vector2Int size)
	{
	}

	private void RenderMeshes()
	{
		List<IMesh> meshes = Clipper.CollectMeshes(this);
		if (meshes.Count == 0)
		{
			return;
		}

		List<ILight>? lights = null;
		foreach (string pass in Passes)
		{
			RenderPass(pass, meshes, ref lights);
		}
	}

	private void RenderPass(string passName, List<IMesh> meshes, ref List<ILight>? lights)
	{
		// collect and sort meshes by order of their render passes
		SortedList<int, (IMesh mesh, Pass pass)> sortedRenderObjects = new(meshes.Count);
		foreach (IMesh mesh in meshes)
		{
			Pass? pass = mesh.Material.FindPassByName(passName);
			if (pass == null)
			{
				continue;
			}

			sortedRenderObjects.Add(pass.Order, (mesh, pass)); // TODO - combine by material to minimize pipeline changes
		}
		
		// draw
		foreach ((IMesh mesh, Pass pass) renderObject in sortedRenderObjects.Values)
		{
			
			
			renderObject.pass.SetupDrawing(_commandList); // TODO - cache
			renderObject.mesh.Draw(_commandList);
		}

		//lights = Clipper.CollectLights(this);
	}
}