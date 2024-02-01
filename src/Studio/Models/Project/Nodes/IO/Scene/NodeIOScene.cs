using Assimp;
using Assimp.Configs;
using Mine.Framework;
using OdinSerializer;
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
		Assimp.Scene? assimpScene = Load(); // TODO - this is wrong
		if (assimpScene == null)
		{
			ConsoleViewModel.LogError($"Cannot import '{Owner.RelativePath}'. Mesh was not loaded!");
			return;
		}

		Mine.Framework.Scene scene = new();
		
		// meshes
		if (assimpScene.HasMeshes)
		{
			scene.Meshes = new List<SceneMesh>();
			for (int i = 0; i < assimpScene.Meshes.Count; ++i)
			{
				Mesh assimpMesh = assimpScene.Meshes[i];

				SceneMesh mesh = new();
				scene.Meshes.Add(mesh);

				if (assimpMesh.PrimitiveType != PrimitiveType.Triangle)
				{
					continue; // ignore this mesh and keep empty in scene
				}

				if (assimpMesh.HasVertices)
				{
					mesh.Vertices = assimpMesh.Vertices.Select(v => new Point3Float(v.X, v.Y, v.Z)).ToList();
				}

				if (assimpMesh.HasNormals)
				{
					mesh.Normals = assimpMesh.Normals.Select(n => new Vector3Float(n.X, n.Y, n.Z)).ToList();
				}

				if (assimpMesh.HasTangentBasis)
				{
					mesh.Tangents = assimpMesh.Tangents.Select(t => new Vector3Float(t.X, t.Y, t.Z)).ToList();
					mesh.BiTangents = assimpMesh.BiTangents.Select(b => new Vector3Float(b.X, b.Y, b.Z)).ToList();
				}

				// TODO - rest

				if (assimpMesh.HasFaces)
				{
					if (mesh.Vertices is not null && mesh.Vertices.Count <= 0xffff)
					{
						mesh.UShortIndices = assimpMesh.GetUnsignedIndices().Select(idx => (ushort)idx).ToArray();
					}
					else
					{
						mesh.UIntIndices = assimpMesh.GetUnsignedIndices();
					}
				}
			}
		}

		byte[] json = SerializationUtility.SerializeValue(scene, DataFormat.Binary);
		File.WriteAllBytes($"{resourcePath}.scene", json);
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