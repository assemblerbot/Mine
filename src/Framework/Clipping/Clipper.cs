namespace Mine.Framework;

public abstract class Clipper
{
	public abstract List<IMesh> CollectMeshes(IRenderer renderer);
	public abstract List<ILight> CollectLights(IRenderer renderer);
}