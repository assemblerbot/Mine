namespace Mine.Studio;

public static class ImporterScenePrefab
{
	public static void CreatePrefab(string path, ProjectNode node)
	{
		try
		{
			string prefabName = Path.GetFileNameWithoutExtension(path);

			ImporterSceneSettings? settings   = node.Meta?.ImporterSettings as ImporterSceneSettings;
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
			writer.WriteLine("		Entity? instance = PrefabUtility.Instantiate(asset);");
			writer.WriteLine("		if (instance is null)");
			writer.WriteLine("		{");
			writer.WriteLine($"			throw new NullReferenceException(\"Asset with id '{node.Meta!.Guid}' is missing!\");");
			writer.WriteLine("		}");
			WriteNodeRecursive(writer, settings, settings.Root, "");
			writer.WriteLine("		parent.AddChild(instance);");
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

	private static void WriteNodeRecursive(StreamWriter writer, ImporterSceneSettings settings, ImporterSceneHierarchyNodeSettings importerSceneHierarchyNode, string thisPath)
	{
		if (!string.IsNullOrEmpty(thisPath))
		{
			if (importerSceneHierarchyNode.Meshes.Count == 0)
			{
				writer.WriteLine($"		// instance.GetChild(\"{thisPath}\")!");
			}
			else
			{
				writer.WriteLine($"		instance.GetChild(\"{thisPath}\")!");
				for (int i = 0; i < importerSceneHierarchyNode.Meshes.Count; ++i)
				{
					string meshName     = settings.Meshes[importerSceneHierarchyNode.Meshes[i]].Name;
					string materialName = settings.Materials[settings.Meshes[importerSceneHierarchyNode.Meshes[i]].MaterialIndex].Name;
					
					writer.Write(i > 0 ? "			.Entity" : "			");
					writer.WriteLine($".AddComponent(new MeshComponent(asset, {importerSceneHierarchyNode.Meshes[i]}, null, ulong.MaxValue)) // mesh:{meshName} material:{materialName}");
				}

				writer.WriteLine("			;");
			}
		}

		if (importerSceneHierarchyNode.Children is null)
		{
			return;
		}

		foreach (ImporterSceneHierarchyNodeSettings child in importerSceneHierarchyNode.Children)
		{
			WriteNodeRecursive(writer, settings, child, $"{thisPath}/{child.Name}");
		}
	}
}