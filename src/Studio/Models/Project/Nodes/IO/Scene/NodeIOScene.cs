using Assimp;
using Assimp.Configs;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.Project.Imports;
using RedHerring.Studio.Models.ViewModels.Console;

namespace Mine.Studio.Scene;

[NodeIO(ProjectNodeType.AssetMesh)]
public sealed class NodeIOScene : NodeIO
{
	private AssimpContext? _context = null;
	private Assimp.Scene?  _scene   = null;
	
	public NodeIOScene(ProjectNode owner) : base(owner)
	{
	}

	public override void Update()
	{
	}

	public override void Load()
	{
		NodeIOSceneSettings? settings = Owner.Meta?.NodeIOSettings as NodeIOSceneSettings;
		if (settings == null)
		{
			return;
		}

		if (_context != null && _scene != null)
		{
			return;
		}

		_context = new AssimpContext();
		_context.SetConfig(new NormalSmoothingAngleConfig(66.0f)); // just for testing

		_scene = _context.ImportFile(
			Owner.AbsolutePath,
			PostProcessSteps.Triangulate
		);
		
		// if(!_scene.HasMeshes) // should be covered by the loop
		// {
		// 	settings.Meshes.Clear();
		// 	return;
		// }
		
		bool settingsChanged = false;
		for (int i = 0; i < _scene.Meshes.Count; ++i)
		{
			// update settings
			Assimp.Mesh assimpMesh = _scene.Meshes[i];
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
		if (settings.Meshes.Count > _scene.Meshes.Count)
		{
			settings.Meshes.RemoveRange(_scene.Meshes.Count, settings.Meshes.Count - _scene.Meshes.Count);
			settingsChanged = true;
		}

		//return settingsChanged ? ImporterResult.FinishedSettingsChanged : ImporterResult.Finished;
	}

	public override void Save()
	{
		throw new InvalidOperationException();
	}

	public override void ClearCache()
	{
		_context = null;
		_scene   = null;
	}

	public override void Import(string resourcePath)
	{
		Load();
		if (_scene == null)
		{
			ConsoleViewModel.LogError($"Cannot import '{Owner.RelativePath}'. Mesh was not loaded!");
			return;
		}

		//Directory.CreateDirectory(Path.GetDirectoryName(resourcePath)!);

		for (int i = 0; i < _scene.Meshes.Count; ++i)
		{
			/* TODO

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
		return new NodeIOSceneSettings();
	}
}