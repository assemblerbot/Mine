namespace Mine.Studio;

public static class NodeIOScenePrefab
{
	public static void CreatePrefab(string path, ProjectNode node)
	{
		try
		{
			string prefabName = Path.GetFileNameWithoutExtension(path);

			NodeIOSceneSettings? settings   = node.Meta?.NodeIOSettings as NodeIOSceneSettings;
			if (settings is null)
			{
				ConsoleViewModel.LogError("Import settings are missing! Cannot create prefab.");
				return;
			}

			using FileStream   stream     = new(path, FileMode.Create);
			using StreamWriter writer     = new(stream);

			writer.WriteLine("// PREFAB");
			writer.WriteLine("using Mine.Framework;");
			writer.WriteLine();
			//writer.WriteLine($"namespace {StudioModel.Instance.Project.}");
			writer.WriteLine($"public static class {prefabName}");
			writer.WriteLine("{");
			writer.WriteLine("	public static Entity Instantiate(Entity parent)");
			writer.WriteLine("	{");

			writer.WriteLine($"		Entity instance = Engine.World.Instantiate(AssetDatabase.Assets[\"{node.Meta!.Guid}\"] as SceneReference, parent);");
			WriteNodeRecursive(writer, settings.Root, "");
			writer.WriteLine("		return instance;");
			
			writer.WriteLine("	}");
			writer.WriteLine("}");
			
			writer.Flush();
			stream.Flush();
		}
		catch (Exception e)
		{
			ConsoleViewModel.LogException(e.ToString());
		}
	}

	private static void WriteNodeRecursive(StreamWriter writer, NodeIOSceneHierarchyNodeSettings node, string thisPath)
	{
		if (!string.IsNullOrEmpty(thisPath))
		{
			writer.WriteLine($"		// instance.Child(\"{thisPath}\")");
		}

		if (node.Children is null)
		{
			return;
		}

		foreach (NodeIOSceneHierarchyNodeSettings child in node.Children)
		{
			WriteNodeRecursive(writer, child, $"{thisPath}/{child.Name}");
		}
	}
}