namespace Mine.Framework;

public sealed class Shared : IDisposable
{
	private Dictionary<(string, int), SharedMesh>     _meshes    = new();
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

	public SharedMesh GetOrCreateMesh(string reference, int meshIndex, SceneMesh sceneMesh)
	{
		if (_meshes.TryGetValue((reference, meshIndex), out SharedMesh? mesh))
		{
			return mesh;
		}

		mesh = new SharedMesh();
		mesh.Init(sceneMesh);
		_meshes.Add((reference,meshIndex), mesh);
		return mesh;
	}

	private static void DisposeDictionary<TKey, T>(Dictionary<TKey, T> dictionary)
		where T : IDisposable
		where TKey : notnull
	{
		foreach (T disposable in dictionary.Values)
		{
			disposable.Dispose();
		}

		dictionary.Clear();
	}
}