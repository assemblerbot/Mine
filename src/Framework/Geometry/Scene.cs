namespace Mine.Framework;

[Serializable]
public sealed class Scene
{
	public SceneNode? Root;
	public List<SceneMesh>? Meshes;
}