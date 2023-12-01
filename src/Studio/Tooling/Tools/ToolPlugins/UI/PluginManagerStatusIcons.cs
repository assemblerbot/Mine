
using System.Numerics;
using Mine.Framework;

namespace Mine.Studio;

public static class PluginManagerStatusIcons
{
    public static readonly string  IconUpToDate      = FontAwesome6.SquareCheck;
    public static          Vector4 IconUpToDateColor = new Vector4(0, 1, 0, 1);

    public static string  IconCanUpgrade    = FontAwesome6.SquarePlus;
    public static Vector4 IconCanUpgradeColor = new Vector4(1, 1, 0, 1);
    
    public static readonly string  IconCanDowngradeOrOverwrite      = FontAwesome6.SquareMinus;
    public static          Vector4 IconCanDowngradeOrOverwriteColor = new Vector4(1, 0.5f, 0, 1);

    public static readonly string  IconNotInstalled      = FontAwesome6.Square;
    public static          Vector4 IconNotInstalledColor = new Vector4(0.5f, 0.5f, 0.5f, 1);
    
    public static string  IconError      = FontAwesome6.SquareXmark;
    public static Vector4 IconErrorColor = new Vector4(1, 0, 0, 1);

    public static void GetPluginStatusIcon(PluginManagerPlugin? repositoryPlugin, PluginManagerPlugin? projectPlugin, out string icon, out Vector4 color)
    {
        if (projectPlugin == null)
        {
            icon  = IconNotInstalled;
            color = IconNotInstalledColor;
            return;
        }

        if (projectPlugin.Error != null)
        {
            icon  = IconError;
            color = IconErrorColor;
            return;
        }

        if (repositoryPlugin == null)
        {
            icon  = IconCanDowngradeOrOverwrite;
            color = IconCanDowngradeOrOverwriteColor;
            return;
        }

        if (repositoryPlugin.Error != null)
        {
            icon  = IconError;
            color = IconErrorColor;
            return;
        }
        
        SimplePluginManagerVersion repository_version_number = repositoryPlugin.ParseVersion;
        SimplePluginManagerVersion project_version_number    = projectPlugin.ParseVersion;

        if (repository_version_number == project_version_number)
        {
            icon  = IconUpToDate;
            color = IconUpToDateColor;
            return;
        }
            
        if (repository_version_number > project_version_number)
        {
            icon  = IconCanUpgrade;
            color = IconCanUpgradeColor;
            return;
        }

        icon  = IconCanDowngradeOrOverwrite;
        color = IconCanDowngradeOrOverwriteColor;
    }

    public static void GetDependencyStatusIcon(bool project_contains_dependency, bool repository_contains_dependency, out string icon, out Vector4 color)
    {
        if (project_contains_dependency)
        {
            icon  = IconUpToDate;
            color = IconUpToDateColor;
            return;
        }

        if (repository_contains_dependency)
        {
            icon  = IconNotInstalled;
            color = IconNotInstalledColor;
            return;
        }

        icon  = IconError;
        color = IconErrorColor;
    }
}
