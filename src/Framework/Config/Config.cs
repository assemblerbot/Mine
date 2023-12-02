using System.Text.Json;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Mine.Framework;

public sealed class Config
{
	private const string ConfigFileName = "engine_config.json";
	private       string ConfigFilePath => Path.Combine(Engine.ApplicationDataPath, ConfigFileName);
	
	public void Load()
	{
		if(!File.Exists(ConfigFilePath))
		{
			return;
		}

		ConfigData? data;
		try
		{
			string json = File.ReadAllText(ConfigFilePath);

			JsonSerializerOptions options = new()
			                                {
				                                IncludeFields = true
			                                };
			
			data = JsonSerializer.Deserialize<ConfigData>(json, options);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return;
		}

		if (data == null)
		{
			return;
		}

		Engine.Window.Position     = new Vector2D<int>(data.WindowPositionX, data.WindowPositionY);
		Engine.Window.Size         = new Vector2D<int>(data.WindowSizeX,     data.WindowSizeY);
		Engine.Window.WindowState  = Enum.TryParse(data.WindowState,  out WindowState windowState) ? windowState : WindowState.Normal;
		Engine.Window.WindowBorder = Enum.TryParse(data.WindowBorder, out WindowBorder windowBorder) ? windowBorder : WindowBorder.Resizable;
	}
	
	public void Save()
	{
		if (!Directory.Exists(Engine.ApplicationDataPath))
		{
			Directory.CreateDirectory(Engine.ApplicationDataPath);
		}

		ConfigData data = new()
		                  {
			                  WindowState     = Engine.Window.WindowState.ToString(),
			                  WindowBorder    = Engine.Window.WindowBorder.ToString(),
			                  WindowPositionX = Engine.Window.Position.X,
			                  WindowPositionY = Engine.Window.Position.Y,
			                  WindowSizeX     = Engine.Window.Size.X,
			                  WindowSizeY     = Engine.Window.Size.Y,
		                  };

		try
		{
			JsonSerializerOptions options = new()
			                                {
				                                IncludeFields = true
			                                };
			
			string json = JsonSerializer.Serialize(data, options);
			File.WriteAllText(ConfigFilePath, json);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return;
		}
	}
}