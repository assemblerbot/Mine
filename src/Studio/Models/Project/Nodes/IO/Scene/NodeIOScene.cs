using Assimp;
using Assimp.Configs;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;
using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio.Scene;

[NodeIO(ProjectNodeType.AssetScene)]
public sealed class NodeIOScene : NodeIO<Assimp.Scene>
{
	public NodeIOScene(ProjectNode owner) : base(owner)
	{
	}

	public override void UpdateCache()
	{
	}

	public override Assimp.Scene? Load()
	{
		NodeIOSceneSettings? settings = Owner.Meta?.NodeIOSettings as NodeIOSceneSettings;
		if (settings == null)
		{
			return null;
		}

		AssimpContext context = new AssimpContext();
		context.SetConfig(new NormalSmoothingAngleConfig(66.0f)); // just for testing

		Assimp.Scene? scene = context.ImportFile(
			Owner.AbsolutePath,
			PostProcessSteps.Triangulate
		);
		
		// if(!_scene.HasMeshes) // should be covered by the loop
		// {
		// 	settings.Meshes.Clear();
		// 	return;
		// }
		
		bool settingsChanged = false;
		for (int i = 0; i < scene.Meshes.Count; ++i)
		{
			// update settings
			Assimp.Mesh assimpMesh = scene.Meshes[i];
			if (i == settings.Meshes.Count)
			{
				settings.Meshes.Add(new NodeIOMeshSettings(assimpMesh.Name));
				settingsChanged = true;
			}
			else if(settings.Meshes[i].Name != assimpMesh.Name)
			{
				settings.Meshes[i] = new NodeIOMeshSettings(assimpMesh.Name);
				settingsChanged    = true;
			}
		}

		// cut the rest
		if (settings.Meshes.Count > scene.Meshes.Count)
		{
			settings.Meshes.RemoveRange(scene.Meshes.Count, settings.Meshes.Count - scene.Meshes.Count);
			settingsChanged = true;
		}

		//return settingsChanged ? ImporterResult.FinishedSettingsChanged : ImporterResult.Finished;
		return scene;
	}

	public override void Save(Assimp.Scene data)
	{
		throw new InvalidOperationException();
	}

	public override void ClearCache()
	{
	}

	public override void Import(string resourcePath)
	{
		Assimp.Scene? scene = Load();
		if (scene == null)
		{
			ConsoleViewModel.LogError($"Cannot import '{Owner.RelativePath}'. Mesh was not loaded!");
			return;
		}

		//Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);

		for (int i = 0; i < scene.Meshes.Count; ++i)
		{
/*
			// import
			Model model = new();

			model.Vertices = new(assimpMesh.VertexCount);
			foreach (Assimp.Vector3D vertex in assimpMesh.Vertices)
			{
				model.Vertices.Add(new Vector3D<float>(vertex.X, vertex.Y, vertex.Z));
			}

			model.Normals = new(assimpMesh.VertexCount);
			foreach (Assimp.Vector3D normal in assimpMesh.Normals)
			{
				model.Normals.Add(new Vector3D<float>(normal.X, normal.Y, normal.Z));
			}

			model.Indices = new(assimpMesh.Faces.Count * 3);
			foreach(Assimp.Face face in assimpMesh.Faces)
			{
				model.Indices.Add(face.Indices[0]);
				model.Indices.Add(face.Indices[1]);
				model.Indices.Add(face.Indices[2]);
			}

			Mesh mesh = new()
			            {
				            Name          = assimpMesh.Name,
				            MaterialIndex = 0,
				            IndexStart    = 0,
				            TriStart      = 0,
				            TriCount      = model.Indices.Count / 3
			            };
			model.Meshes = new() { mesh };

			byte[] json = SerializationUtility.SerializeValue(model, DataFormat.JSON);
			File.WriteAllBytes($"{resourcePath}_{mesh.Name}_.mesh", json);
*/			
		}
		
		ClearCache();
	}

	public override NodeIOSettings CreateImportSettings()
	{
		NodeIOSceneSettings settings = new NodeIOSceneSettings();
		UpdateImportSettings(settings);
		return settings;
	}

	public override bool UpdateImportSettings(NodeIOSettings settings)
	{
		NodeIOSceneSettings? sceneSettings = settings as NodeIOSceneSettings;
		if (sceneSettings == null)
		{
			return false;
		}

		AssimpContext context = new AssimpContext();
		//context.SetConfig(new NormalSmoothingAngleConfig(66.0f)); // just for testing

		Assimp.Scene? scene = context.ImportFile(
			Owner.AbsolutePath,
			PostProcessSteps.Triangulate
		);
		
		// if(!_scene.HasMeshes) // should be covered by the loop
		// {
		// 	settings.Meshes.Clear();
		// 	return;
		// }

		Node? root = scene.RootNode;
		
		bool settingsChanged = false;
		for (int i = 0; i < scene.Meshes.Count; ++i)
		{
			// update settings
			Assimp.Mesh assimpMesh = scene.Meshes[i];
			if (i == sceneSettings.Meshes.Count)
			{
				sceneSettings.Meshes.Add(new NodeIOMeshSettings(assimpMesh.Name));
				settingsChanged = true;
			}
			else if(sceneSettings.Meshes[i].Name != assimpMesh.Name)
			{
				sceneSettings.Meshes[i] = new NodeIOMeshSettings(assimpMesh.Name);
				settingsChanged    = true;
			}
		}

		// cut the rest
		if (sceneSettings.Meshes.Count > scene.Meshes.Count)
		{
			sceneSettings.Meshes.RemoveRange(scene.Meshes.Count, sceneSettings.Meshes.Count - scene.Meshes.Count);
			settingsChanged = true;
		}

		return settingsChanged;
	}
}