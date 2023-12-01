using System.Text.Json;
using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

public class PluginManagerCollection
{
    private const string _pluginFileName  = "plugin.json";
    private const string _packageFileName = "package.json";
        
    private Dictionary<string, PluginManagerPlugin> _plugins = null!;
    public  Dictionary<string, PluginManagerPlugin> Plugins => _plugins;
        
    public string? Init(string path)
    {
        _plugins = new Dictionary<string, PluginManagerPlugin>();
            
        if (!Directory.Exists(path))
        {
            return null;
        }

        string? errorMessage = null;
            
        // plugins
        {
            string[] descriptors = Directory.GetFiles(path, _pluginFileName, SearchOption.AllDirectories);
            foreach (string descriptor in descriptors)
            {
                PluginManagerPlugin plugin = ReadPlugin(descriptor);
                AddPlugin(plugin, ref errorMessage);
            }
        }
            
        return errorMessage;
    }

    private void AddPlugin(PluginManagerPlugin plugin, ref string error_message)
    {
        if (_plugins.ContainsKey(plugin.Id))
        {
            ConsoleViewModel.Log($"Duplicate plugin id '{plugin.Id}' detected! Is there a wrong path in config file?", ConsoleItemType.Error);
            error_message = "One or more duplicate plugin ids detected! See log for details.";
            return;
        }
            
        _plugins.Add(plugin.Id, plugin);
    }
        
    private PluginManagerPlugin ReadPlugin(string path)
    {
        PluginManagerPlugin? plugin;
            
        try
        {
            string json = File.ReadAllText(path);
            JsonSerializerOptions options = new()
                                            {
                                                IncludeFields = true
                                            };
            plugin = JsonSerializer.Deserialize<PluginManagerPlugin>(json, options);
            if (plugin == null)
            {
                throw new InvalidDataException("Plugin descriptor parsing failed!");
            }
        }
        catch (Exception e)
        {
            plugin = new PluginManagerPlugin{Id = path, Name = path, Error = e.ToString()};
        }

        plugin.PathToPlugin = path;
        return plugin;
    }
}