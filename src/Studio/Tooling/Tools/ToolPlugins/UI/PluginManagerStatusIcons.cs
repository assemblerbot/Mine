using System.Numerics;
using Mine.Framework;
using Mine.ImGuiPlugin;

namespace Mine.Studio;

public static class PluginManagerStatusIcons
{
	private const           string  _iconUpToDate      = FontAwesome6.SquareCheck;
	private static readonly Vector4 _iconUpToDateColor = new (0, 1, 0, 1);

	private const           string  _iconCanUpgrade      = FontAwesome6.SquarePlus;
	private static readonly Vector4 _iconCanUpgradeColor = new (1, 1, 0, 1);

	private const           string  _iconCanDowngradeOrOverwrite      = FontAwesome6.SquareMinus;
	private static readonly Vector4 _iconCanDowngradeOrOverwriteColor = new (1, 0.5f, 0, 1);

	private const           string  _iconNotInstalled      = FontAwesome6.Square;
	private static readonly Vector4 _iconNotInstalledColor = new (0.5f, 0.5f, 0.5f, 1);

	private const           string  _iconError      = FontAwesome6.SquareXmark;
	private static readonly Vector4 _iconErrorColor = new (1, 0, 0, 1);

	public static void GetPluginStatusIcon(PluginManagerPlugin? repositoryPlugin, PluginManagerPlugin? projectPlugin, out string icon, out Vector4 color)
	{
		if (projectPlugin == null)
		{
			icon  = _iconNotInstalled;
			color = _iconNotInstalledColor;
			return;
		}

		if (projectPlugin.Error != null)
		{
			icon  = _iconError;
			color = _iconErrorColor;
			return;
		}

		if (repositoryPlugin == null)
		{
			icon  = _iconCanDowngradeOrOverwrite;
			color = _iconCanDowngradeOrOverwriteColor;
			return;
		}

		if (repositoryPlugin.Error != null)
		{
			icon  = _iconError;
			color = _iconErrorColor;
			return;
		}
        
		PluginManagerVersion repositoryVersionNumber = repositoryPlugin.ParseVersion;
		PluginManagerVersion projectVersionNumber    = projectPlugin.ParseVersion;

		if (repositoryVersionNumber == projectVersionNumber)
		{
			icon  = _iconUpToDate;
			color = _iconUpToDateColor;
			return;
		}
            
		if (repositoryVersionNumber > projectVersionNumber)
		{
			icon  = _iconCanUpgrade;
			color = _iconCanUpgradeColor;
			return;
		}

		icon  = _iconCanDowngradeOrOverwrite;
		color = _iconCanDowngradeOrOverwriteColor;
	}

	public static void GetDependencyStatusIcon(bool projectContainsDependency, bool repositoryContainsDependency, out string icon, out Vector4 color)
	{
		if (projectContainsDependency)
		{
			icon  = _iconUpToDate;
			color = _iconUpToDateColor;
			return;
		}

		if (repositoryContainsDependency)
		{
			icon  = _iconNotInstalled;
			color = _iconNotInstalledColor;
			return;
		}

		icon  = _iconError;
		color = _iconErrorColor;
	}
}