namespace Mine.Framework;

public class RendererComponent : Component, IRenderer
{
	public int          RenderOrder { get; private set; }
	public ulong        RenderMask  { get; private set; }
	public Clipper      Clipper;
	public List<string> Passes = new();

	public RendererComponent(int renderOrder, ulong renderMask, Clipper clipper)
	{
		RenderOrder = renderOrder;
		RenderMask  = renderMask;
		Clipper     = clipper;
	}

	public override void AfterAddedToWorld()
	{
		Engine.World.RegisterRenderer(this);
	}

	public override void BeforeRemovedFromWorld()
	{
		Engine.World.UnregisterRenderer(this);
	}

	public virtual void Render()
	{
		List<IMesh>   meshes = Clipper.CollectMeshes(this);
		List<ILight>? lights = null;

		foreach (string pass in Passes)
		{
			RenderPass(pass, meshes, ref lights);
		}
	}
	
	public virtual void WindowResized(Vector2Int size)
	{
		
	}

	private void RenderPass(string passName, List<IMesh> meshes, ref List<ILight>? lights)
	{
		// collect and sort meshes by order of their render passes
		SortedList<int, (IMesh, Pass)> sortedMeshes = new(meshes.Count);
		foreach (IMesh mesh in meshes)
		{
			Pass? pass = mesh.Material.FindPassByName(passName);
			if (pass == null)
			{
				continue;
			}

			sortedMeshes.Add(pass.Order, (mesh, pass)); // TODO - combine by material to minimize pipeline changes
		}
		
		// draw
		foreach ((IMesh, Pass) item in sortedMeshes.Values)
		{
			
		}

		//lights = Clipper.CollectLights(this);
	}
}