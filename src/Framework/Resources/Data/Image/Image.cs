namespace Mine.Framework;

public class Image
{
	public readonly int    Width;
	public readonly int    Height;
	public readonly uint[] Pixels32BitRGBA;

	public Image(int width, int height, uint[] pixels32BitRgba)
	{
		Width           = width;
		Height          = height;
		Pixels32BitRGBA = pixels32BitRgba;
	}
}