using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio;

public static class PluginManagerUnityUtils
{
    public static void AddGlobalDefine(string define)
    {
        ConsoleViewModel.Log("Defines are not supported yet!", ConsoleItemType.Warning);
        // string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        //
        // if (!symbols.Contains(define))
        // {
        //     PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols + (string.IsNullOrEmpty(symbols) ? "" : ";") + define);
        // }
    }

    public static void RemoveGlobalDefine(string define)
    {
        ConsoleViewModel.Log("Defines are not supported yet!", ConsoleItemType.Warning);
        // string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        //
        // if (symbols.Contains(define))
        // {
        //     symbols = symbols.Replace(";" + define, "");
        //     PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols);
        // }
    }
}