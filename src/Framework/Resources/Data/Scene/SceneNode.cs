namespace Mine.Framework;

[Serializable]
public class SceneNode
{
	public string          Name;
	public Vector3Float    Translation;
	public QuaternionFloat Rotation;
	public Vector3Float    Scale;

	public List<int>? MeshIndices;
	
	public List<SceneNode>? Children;
}