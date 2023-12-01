using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

public static class PluginManagerFileUtils
{
    public static void Log(string message)
    {
        ConsoleViewModel.Log(message, ConsoleItemType.Info);
    }

    public static void CopyPlugin(string sourcePathToDescriptorFile, string sourceRootDirectory, string destinationRootDirectory)
    {
        if (!sourcePathToDescriptorFile.StartsWith(sourceRootDirectory))
        {
            throw new System.Exception("Source path is not inside source root directory!");
        }

        string sourcePluginDirectory = Path.GetDirectoryName(sourcePathToDescriptorFile);

        string relativeSourcePluginDirectory = sourcePluginDirectory.Substring(sourceRootDirectory.Length);
        if (relativeSourcePluginDirectory.First() == '\\' || relativeSourcePluginDirectory.First() == '/')
        {
            relativeSourcePluginDirectory = relativeSourcePluginDirectory.Substring(1);
        }

        string destinationPluginDirectory = Path.Combine(destinationRootDirectory, relativeSourcePluginDirectory);

        Log("======= Copying files =======");
        Log("From: " + sourcePluginDirectory);
        Log("To: "   + destinationPluginDirectory);

        if (Directory.Exists(destinationPluginDirectory))
        {
            try
            {
                Directory.Delete(destinationPluginDirectory, true);
            }
            catch(Exception e)
            {
                ConsoleViewModel.Log(e.ToString(), ConsoleItemType.Exception);
                return;
            }
        }

        CreateDirectoryStructure(destinationPluginDirectory);
        CopyRecursive(sourcePluginDirectory, destinationPluginDirectory);
        Log("DONE!");
    }

    public static void DeletePlugin(string sourcePathToDescriptorFile)
    {
        string sourcePluginDirectory = Path.GetDirectoryName(sourcePathToDescriptorFile);

        Log("======= Deleting directory =======");
        if (Directory.Exists(sourcePluginDirectory))
        {
            Log("Deleting: " + sourcePluginDirectory);
            try
            {
                Directory.Delete(sourcePluginDirectory, true);
            }
            catch(Exception e)
            {
                ConsoleViewModel.Log(e.ToString(), ConsoleItemType.Exception);
                return;
            }
        }

        Log("DONE!");
    }

    public static void CreateDirectoryStructure(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            return;
        }

        CreateDirectoryStructure(Path.GetDirectoryName(directoryPath));
            
        Log($"Creating: {directoryPath}");
        Directory.CreateDirectory(directoryPath);
    }

    public static void CopyRecursive(string fromDirectory, string toDirectory)
    {
        if (!Directory.Exists(toDirectory))
        {
            Log($"Creating: {toDirectory}");
            Directory.CreateDirectory(toDirectory);
        }

        // files
        foreach (string fromFilePath in Directory.GetFiles(fromDirectory))
        {
            string toFilePath = Path.Combine(toDirectory, Path.GetFileName(fromFilePath));
            Log($"Copying: {fromFilePath} -> {toFilePath}");
            File.Copy(fromFilePath, toFilePath);
        }
 
        // directories
        foreach (string fromDirPath in Directory.GetDirectories(fromDirectory))
        {
            string toDirPath = Path.Combine(toDirectory, Path.GetFileName(fromDirPath));
            CopyRecursive(fromDirPath, toDirPath);
        }
    }
}