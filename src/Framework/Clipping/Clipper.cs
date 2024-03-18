namespace Mine.Framework;

public abstract class Clipper
{
	public abstract List<MeshComponent> CollectMeshes(IRenderer renderer);
	public abstract List<LightComponent> CollectLights(IRenderer renderer);
}