namespace Mine.Framework;

[Serializable]
public sealed class ConfigData
{
	public string WindowState     = "Normal";
	public string WindowBorder    = "Resizable";
	public int    WindowPositionX = 100;
	public int    WindowPositionY = 100;
	public int    WindowSizeX     = 640;
	public int    WindowSizeY     = 480;
}