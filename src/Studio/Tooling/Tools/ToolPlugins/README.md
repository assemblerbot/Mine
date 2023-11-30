# Plugin Manager
**Plugin Manager** is a lightweight Unity tool that manages folder based plugins stored on HDD repository.
It can handle dependencies and also supports optional dependencies through global defines in unity project.

## How to make a plugin
* create folder in repository or in plugins folder inside unity project
* add any files you want there
* create **plugin.json**

### plugin.json structure
Example json structure:
```json
{
  "Id":"MyPluginId",
  "Name":"Name visible in manager",
  "Description" : "Description. <b>Bold</b> and <i>italic</i> texts are supported.",
  "Version":"0.1.0",
  "Dependencies":["OtherPluginId"],
  "GlobalDefines":["MY_PLUGIN_INSTALLED"]
}
```
* **Id** must be unique across all your plugins.
* **Name** is visible name, keep it short.
* **Description** is longer plugin description visible after plugin ui is expanded.
* **Version** _**MUST**_ follow **Semantic versioning** standard: `MAJOR.MINOR.PATCH-PRE.RELEASE+META`. See https://semver.org/
* **Dependencies** is optional array of Id's of other plugins, your plugin is dependent on.
* **GlobalDefines** is optional array of global defines that will be added to your unity project settings when plugin is installed (and removed when it's uninstalled). Defines can be used for optional dependencies on the plugin. 

### package.json
If **plugin.json** is not present then **package.json** can be used too.
See [Unity package manifest doc](https://docs.unity3d.com/Manual/upm-manifestPkg.html) for more info.

Notes:
* Version format _**MUST**_ follow the same rules as for plugin.json file in order to work properly.
* If both files: **plugin.json** and **package.json** are present then **plugin.json** is used. 

## Working on plugins
Modify your plugin in your unity project. When you increase version in *plugin.json* manager will offer your to downgrade plugin back from repository or copy new version to repository.
 
