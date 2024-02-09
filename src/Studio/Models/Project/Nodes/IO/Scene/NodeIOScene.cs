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
	public override string ReferenceType => nameof(SceneReference);
	
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

		Assimp.Scene? assimpScene = context.ImportFile(
			Owner.AbsolutePath,
			PostProcessSteps.Triangulate
		);
		
		if(!assimpScene.HasMeshes)
		{
			settings.Meshes.Clear();
			return assimpScene;
		}
		
		bool settingsChanged = false;
		for (int i = 0; i < assimpScene.Meshes.Count; ++i)
		{
			// update settings
			Assimp.Mesh assimpMesh = assimpScene.Meshes[i];
			if (i == settings.Meshes.Count)
			{
				settings.Meshes.Add(new NodeIOSceneMeshSettings(assimpMesh.Name));
				settingsChanged = true;
			}
			else if(settings.Meshes[i].Name != assimpMesh.Name)
			{
				settings.Meshes[i] = new NodeIOSceneMeshSettings(assimpMesh.Name);
				settingsChanged    = true;
			}
		}

		// cut the rest
		if (settings.Meshes.Count > assimpScene.Meshes.Count)
		{
			settings.Meshes.RemoveRange(assimpScene.Meshes.Count, settings.Meshes.Count - assimpScene.Meshes.Count);
			settingsChanged = true;
		}

		//return settingsChanged ? ImporterResult.FinishedSettingsChanged : ImporterResult.Finished;
		return assimpScene;
	}

	public override void Save(Assimp.Scene data)
	{
		throw new InvalidOperationException();
	}

	public override void ClearCache()
	{
	}

	public override string? Import(string resourcesRootPath)
	{
		NodeIOSceneSettings? settings = Owner.Meta?.NodeIOSettings as NodeIOSceneSettings;
		if (settings == null)
		{
			ConsoleViewModel.LogError($"Cannot import '{Owner.RelativePath}'. There are now settings!");
			return null;
		}

		AssimpContext context = new AssimpContext();
		context.SetConfig(new NormalSmoothingAngleConfig(settings.NormalSmoothingAngle));

		Assimp.Scene? assimpScene = context.ImportFile(
			Owner.AbsolutePath,
			PostProcessSteps.Triangulate // always - only triangles are supported
		);

		if (assimpScene == null)
		{
			ConsoleViewModel.LogError($"Cannot import '{Owner.RelativePath}'. Mesh was not loaded!");
			return null;
		}

		Mine.Framework.Scene scene = new();
		
		// meshes ---------------------------------------
		if (assimpScene.HasMeshes)
		{
			scene.Meshes = new List<SceneMesh>();
			for (int i = 0; i < assimpScene.Meshes.Count; ++i)
			{
				Mesh assimpMesh = assimpScene.Meshes[i];

				SceneMesh mesh = new();
				scene.Meshes.Add(mesh);

				mesh.Name        = assimpMesh.Name;
				mesh.BoundingBox = new BoundingBoxFloat(
					new Point3Float(assimpMesh.BoundingBox.Min.X, assimpMesh.BoundingBox.Min.Y, assimpMesh.BoundingBox.Min.Z),
					new Point3Float(assimpMesh.BoundingBox.Max.X, assimpMesh.BoundingBox.Max.Y, assimpMesh.BoundingBox.Max.Z)
				);
				
				if (assimpMesh.PrimitiveType != PrimitiveType.Triangle)
				{
					continue; // ignore this mesh and keep empty in scene
				}
				
				// positions
				if (assimpMesh.HasVertices)
				{
					mesh.Positions = assimpMesh.Vertices.Select(v => new Point3Float(v.X, v.Y, v.Z)).ToList();
				}

				// normals
				if (assimpMesh.HasNormals)
				{
					mesh.Normals = assimpMesh.Normals.Select(n => new Vector3Float(n.X, n.Y, n.Z)).ToList();
				}

				// tangents and bitangents
				if (assimpMesh.HasTangentBasis)
				{
					mesh.Tangents = assimpMesh.Tangents.Select(t => new Vector3Float(t.X, t.Y, t.Z)).ToList();
					mesh.BiTangents = assimpMesh.BiTangents.Select(b => new Vector3Float(b.X, b.Y, b.Z)).ToList();
				}

				// UV
				if (assimpMesh.TextureCoordinateChannelCount > 0)
				{
					mesh.TextureCoordinateChannels = new List<SceneMeshTextureCoordinateChannel>();
					for (int channelIndex = 0; channelIndex < assimpMesh.TextureCoordinateChannelCount; ++channelIndex)
					{
						SceneMeshTextureCoordinateChannel channel = new ();
						
						if (assimpMesh.UVComponentCount[channelIndex] == 2)
						{
							channel.UV = assimpMesh.TextureCoordinateChannels[channelIndex].Select(uv => new Point2Float(uv.X, uv.Y)).ToList();
						}
						else if (assimpMesh.UVComponentCount[channelIndex] == 3)
						{
							channel.UVW = assimpMesh.TextureCoordinateChannels[channelIndex].Select(uv => new Point3Float(uv.X, uv.Y, uv.Z)).ToList();
						}
						else
						{
							continue;
						}

						mesh.TextureCoordinateChannels.Add(channel);
					}
				}
				
				// colors
				if (assimpMesh.VertexColorChannelCount > 0)
				{
					mesh.VertexColorChannels = new List<SceneMeshVertexColorChannel>();
					for (int channelIndex = 0; channelIndex < assimpMesh.VertexColorChannelCount; ++channelIndex)
					{
						SceneMeshVertexColorChannel channel = new();
						mesh.VertexColorChannels.Add(channel);
						channel.Colors = assimpMesh.VertexColorChannels[channelIndex].Select(color => new Color4FloatRGBA(color.R, color.G, color.B, color.A)).ToList();
					}
				}
				
				// TODO - rest: bones, animation, morphing

				// faces
				if (assimpMesh.HasFaces)
				{
					if (mesh.Positions is not null && mesh.Positions.Count <= 0xffff)
					{
						mesh.UShortIndices = assimpMesh.GetUnsignedIndices().Select(idx => (ushort)idx).ToList();
					}
					else
					{
						mesh.UIntIndices = assimpMesh.GetUnsignedIndices().ToList();
					}
				}
			}
		}
		
		// hierarchy ----------------------------------------
		if (assimpScene.RootNode != null)
		{
			scene.Root      = new SceneNode();
			ImportNode(scene.Root, assimpScene.RootNode);
			ImportChildNodesRecursive(scene.Root, assimpScene.RootNode);
		}

		// import
		byte[] json         = SerializationUtility.SerializeValue(scene, DataFormat.Binary);
		string relativePath = $"{Owner.RelativePath}.scene";
		string absolutePath         = Path.Join(resourcesRootPath, relativePath);
		File.WriteAllBytes(absolutePath, json);
		
		return relativePath;
	}

	private void ImportChildNodesRecursive(SceneNode targetNode, Assimp.Node source)
	{
		if (source.Children == null)
		{
			return;
		}

		foreach (Node sourceChild in source.Children)
		{
			targetNode.Children ??= new List<SceneNode>();

			SceneNode child = new();
			ImportNode(child, sourceChild);
			targetNode.Children.Add(child);

			ImportChildNodesRecursive(child, sourceChild);
		}
	}

	private void ImportNode(SceneNode targetNode, Assimp.Node source)
	{
		targetNode.Name = source.Name;

		source.Transform.Decompose(out Vector3D scale, out Quaternion rotation, out Vector3D translation);
		targetNode.Translation = new Vector3Float(translation.X, translation.Y, translation.Z);
		targetNode.Rotation    = new QuaternionFloat(rotation.X, rotation.Y, rotation.Z, rotation.W);
		targetNode.Scale       = new Vector3Float(scale.X, scale.Y, scale.Z);

		if (source.HasMeshes)
		{
			targetNode.MeshIndices = new List<int>();
			for (int i = 0; i < source.MeshIndices.Count; ++i)
			{
				targetNode.MeshIndices.Add(source.MeshIndices[i]);
			}
		}
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

		Assimp.Scene? scene = context.ImportFile(
			Owner.AbsolutePath,
			PostProcessSteps.None
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
				sceneSettings.Meshes.Add(new NodeIOSceneMeshSettings(assimpMesh.Name));
				settingsChanged = true;
			}
			else if(sceneSettings.Meshes[i].Name != assimpMesh.Name)
			{
				sceneSettings.Meshes[i] = new NodeIOSceneMeshSettings(assimpMesh.Name);
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