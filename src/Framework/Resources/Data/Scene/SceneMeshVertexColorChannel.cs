namespace Mine.Framework;

[Serializable]
public sealed class SceneMeshVertexColorChannel
{
	public const int ItemSize = Color4FloatRGBA.Size;
	
	public List<Color4FloatRGBA> Colors = new();
}