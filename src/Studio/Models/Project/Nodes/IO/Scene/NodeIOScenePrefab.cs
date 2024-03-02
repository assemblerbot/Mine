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
			writer.WriteLine($"using {StudioModel.Instance.Project.ProjectSettings.AssetDatabaseNamespace};"); 
			writer.WriteLine("using Mine.Framework;");
			writer.WriteLine();
			//writer.WriteLine($"namespace {StudioModel.Instance.Project.}");
			writer.WriteLine($"public static class {prefabName}");
			writer.WriteLine("{");
			writer.WriteLine("	public static Entity Instantiate(Entity parent)");
			writer.WriteLine("	{");

			writer.WriteLine($"		SceneReference? asset = AssetDatabase.Assets[\"{node.Meta!.Guid}\"] as SceneReference;");
			writer.WriteLine("		Entity? instance = Engine.World.Instantiate(asset, parent);");
			writer.WriteLine("		if (instance is null)");
			writer.WriteLine("		{");
			writer.WriteLine($"			throw new NullReferenceException(\"Asset with id '{node.Meta!.Guid}' is missing!\");");
			writer.WriteLine("		}");
			WriteNodeRecursive(writer, settings, settings.Root, "");
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

	private static void WriteNodeRecursive(StreamWriter writer, NodeIOSceneSettings settings, NodeIOSceneHierarchyNodeSettings node, string thisPath)
	{
		if (!string.IsNullOrEmpty(thisPath))
		{
			if (node.Meshes.Count == 0)
			{
				writer.WriteLine($"		// instance.GetChild(\"{thisPath}\")!");
			}
			else
			{
				writer.WriteLine($"		instance.GetChild(\"{thisPath}\")!");
				for (int i = 0; i < node.Meshes.Count; ++i)
				{
					string meshName     = settings.Meshes[node.Meshes[i]].Name;
					string materialName = settings.Materials[settings.Meshes[node.Meshes[i]].MaterialIndex].Name;
					
					writer.Write(i > 0 ? "			.Entity" : "			");
					writer.WriteLine($".AddComponent(new MeshComponent(asset, {node.Meshes[i]}, null)) // mesh:{meshName} material:{materialName}");
				}

				writer.WriteLine("			;");
			}
		}

		if (node.Children is null)
		{
			return;
		}

		foreach (NodeIOSceneHierarchyNodeSettings child in node.Children)
		{
			WriteNodeRecursive(writer, settings, child, $"{thisPath}/{child.Name}");
		}
	}
}