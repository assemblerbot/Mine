using Veldrid;

namespace Mine.Framework;

public class RendererComponent : Component, IRenderer
{
	public int          RenderOrder { get; private set; }
	public ulong        RenderMask  { get; private set; }
	public Clipper      Clipper;
	public List<string> Passes = new();

	private CommandList _commandList = null!;

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

		_commandList.Dispose();
		_commandList = null!;
	}

	public virtual void Render()
	{
		// set and clear target
		// TODO
		
		if (Passes.Count == 0)
		{
			return;
		}

		List<IMesh> meshes = Clipper.CollectMeshes(this);
		if (meshes.Count == 0)
		{
			return;
		}

		List<ILight>? lights = null;

		_commandList.Begin();
		foreach (string pass in Passes)
		{
			RenderPass(pass, meshes, ref lights);
		}
		_commandList.End();
		Engine.Graphics.Device.SubmitCommands(_commandList);
	}
	
	public virtual void WindowResized(Vector2Int size)
	{
		
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