namespace Mine.Framework;

public sealed class SharedDataManager : IDisposable
{
	private Dictionary<string, SharedMesh>     _meshes    = new();
	private Dictionary<string, SharedMaterial> _materials = new();
	private Dictionary<string, SharedTexture>  _textures  = new();
	private Dictionary<string, SharedPipeline> _pipelines = new();

	public void Dispose()
	{
		DisposeDictionary(_pipelines);
		DisposeDictionary(_meshes);
		DisposeDictionary(_materials);
		DisposeDictionary(_textures);
	}

	public SharedMesh GetOrCreateMesh(string reference, SceneMesh sceneMesh)
	{
		if (_meshes.TryGetValue(reference, out SharedMesh? mesh))
		{
			return mesh;
		}

		mesh = new SharedMesh();
		mesh.Init(sceneMesh);
		_meshes.Add(reference, mesh);
		return mesh;
	}

	private static void DisposeDictionary<T>(Dictionary<string, T> dictionary) where T : IDisposable
	{
		foreach (T disposable in dictionary.Values)
		{
			disposable.Dispose();
		}

		dictionary.Clear();
	}
}