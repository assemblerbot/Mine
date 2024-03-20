namespace Mine.Framework;

// clip objects based on render mask only
public class MaskClipper : Clipper
{
	public override List<MeshComponent> CollectMeshes(IRenderer renderer)
	{
		return Collect(renderer, Engine.World.Meshes);
	}

	public override List<LightComponent> CollectLights(IRenderer renderer)
	{
		return Collect(renderer, Engine.World.Lights);
	}

	private static List<T> Collect<T>(IRenderer renderer, IReadOnlyList<T> list) where T : IRenderable
	{
		List<T> result = new();

		foreach (T item in list)
		{
			if ((item.RenderMask & renderer.RenderMask) != 0)
			{
				result.Add(item);
			}
		}

		return result;
	}
}