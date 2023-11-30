using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

public class SimplePluginManagerCollection
{
    private const string c_PluginFileName  = "plugin.json";
    private const string c_PackageFileName = "package.json";
        
    private Dictionary<string, PluginManagerPlugin> m_Plugins;
    public  Dictionary<string, PluginManagerPlugin> Plugins => m_Plugins;
        
    public string? Init(string path)
    {
        m_Plugins = new Dictionary<string, PluginManagerPlugin>();
            
        if (!Directory.Exists(path))
        {
            return null;
        }

        string? errorMessage = null;
            
        // plugins
        {
            string[] descriptors = Directory.GetFiles(path, c_PluginFileName, SearchOption.AllDirectories);
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
        if (m_Plugins.ContainsKey(plugin.Id))
        {
            ConsoleViewModel.Log($"Duplicate plugin id '{plugin.Id}' detected! Is there a wrong path in config file?", ConsoleItemType.Error);
            error_message = "One or more duplicate plugin ids detected! See log for details.";
            return;
        }
            
        m_Plugins.Add(plugin.Id, plugin);
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