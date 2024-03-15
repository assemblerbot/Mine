using Veldrid;

namespace Mine.Framework;

public sealed class Shared : IDisposable
{
	private readonly Dictionary<(string, int), SharedMesh> _meshes        = new();
	private readonly Dictionary<string, SharedMaterial>    _materials     = new();
	private readonly Dictionary<string, SharedTexture>     _textures      = new();
	private readonly Dictionary<string, SharedPipeline>    _pipelines     = new();
	private readonly List<SharedVertexLayout>              _vertexLayouts = new();
	//private readonly List<

	public void Dispose()
	{
		DisposeDictionary(_pipelines);
		DisposeDictionary(_meshes);
		DisposeDictionary(_materials);
		DisposeDictionary(_textures);
		_vertexLayouts.Clear();
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

	public SharedVertexLayout GetOrCreateVertexLayout(VertexLayoutDescription description)
	{
		int index = _vertexLayouts.FindIndex(x => x.VertexLayoutDescription.Equals(description));
		if (index != -1)
		{
			return _vertexLayouts[index];
		}

		SharedVertexLayout layout = new (Engine.NextUniqueId, description);
		_vertexLayouts.Add(layout);
		return layout;
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