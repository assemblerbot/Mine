namespace Mine.Framework;

[Serializable]
public sealed class SceneMeshVertexColorChannel
{
	public const int ItemSizeInBytes = Color4FloatRGBA.SizeInBytes;
	
	public List<Color4FloatRGBA> Colors = new();
}