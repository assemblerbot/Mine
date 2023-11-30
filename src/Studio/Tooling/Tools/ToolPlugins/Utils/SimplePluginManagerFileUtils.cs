using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

public static class SimplePluginManagerFileUtils
{
    public static void Log(string message)
    {
        ConsoleViewModel.Log(message, ConsoleItemType.Info);
    }

    public static void CopyPlugin(string source_path_to_descriptor_file, string source_root_directory, string destination_root_directory)
    {
        if (!source_path_to_descriptor_file.StartsWith(source_root_directory))
        {
            throw new System.Exception("Source path is not inside source root directory!");
        }

        string source_plugin_directory = Path.GetDirectoryName(source_path_to_descriptor_file);

        string relative_source_plugin_directory = source_plugin_directory.Substring(source_root_directory.Length);
        if (relative_source_plugin_directory.First() == '\\' || relative_source_plugin_directory.First() == '/')
        {
            relative_source_plugin_directory = relative_source_plugin_directory.Substring(1);
        }

        string destination_plugin_directory = Path.Combine(destination_root_directory, relative_source_plugin_directory);

        Log("======= Copying files =======");
        Log("From: " + source_plugin_directory);
        Log("To: "   + destination_plugin_directory);

        if (Directory.Exists(destination_plugin_directory))
        {
            Directory.Delete(destination_plugin_directory, true);
        }

        CreateDirectoryStructure(destination_plugin_directory);
        CopyRecursive(source_plugin_directory, destination_plugin_directory);
        Log("DONE!");
    }

    public static void DeletePlugin(string source_path_to_descriptor_file)
    {
        string source_plugin_directory = Path.GetDirectoryName(source_path_to_descriptor_file);

        Log("======= Deleting directory =======");
        if (Directory.Exists(source_plugin_directory))
        {
            Log("Deleting: " + source_plugin_directory);
            Directory.Delete(source_plugin_directory, true);
        }

        Log("DONE!");
    }

    public static void CreateDirectoryStructure(string directory_path)
    {
        if (Directory.Exists(directory_path))
        {
            return;
        }

        CreateDirectoryStructure(Path.GetDirectoryName(directory_path));
            
        Log($"Creating: {directory_path}");
        Directory.CreateDirectory(directory_path);
    }

    public static void CopyRecursive(string from_directory, string to_directory)
    {
        if (!Directory.Exists(to_directory))
        {
            Log($"Creating: {to_directory}");
            Directory.CreateDirectory(to_directory);
        }

        // files
        foreach (string from_file_path in Directory.GetFiles(from_directory))
        {
            string to_file_path = Path.Combine(to_directory, Path.GetFileName(from_file_path));
            Log($"Copying: {from_file_path} -> {to_file_path}");
            File.Copy(from_file_path, to_file_path);
        }
 
        // directories
        foreach (string from_dir_path in Directory.GetDirectories(from_directory))
        {
            string to_dir_path = Path.Combine(to_directory, Path.GetFileName(from_dir_path));
            CopyRecursive(from_dir_path, to_dir_path);
        }
    }
}