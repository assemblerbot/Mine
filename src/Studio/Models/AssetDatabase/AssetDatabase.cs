using System.Text.Json.Serialization;
using Mine.Framework;
using OdinSerializer;

namespace Mine.Studio;

[Serializable]
public sealed class AssetDatabase
{
	private const string     FILE_NAME = "resources.bin";
	private const DataFormat FORMAT    = DataFormat.JSON;
	
	[OdinSerialize] private List<AssetDatabaseItem> _items = new(); // just for serialization
	
	[JsonIgnore] private Dictionary<string, AssetDatabaseItem>? _indexedItems;
	[JsonIgnore] private bool                                   _dirty = false;
	[JsonIgnore] public  bool                                   IsDirty => _dirty;

	[JsonIgnore] public AssetDatabaseItem? this[string key]
	{
		get => InitIndexation().TryGetValue(key, out AssetDatabaseItem? path) ? path : null;
		set
		{
			if (value == null)
			{
				throw new NullReferenceException("Asset database item cannot be null!");
			}

			InitIndexation()[key] = value;
			_dirty                = true;
		}
	}

	private Dictionary<string, AssetDatabaseItem> InitIndexation()
	{
		if (_indexedItems is not null)
		{
			return _indexedItems;
		}

		if (_items is null)
		{
			_items = new();
		}

		_indexedItems = new Dictionary<string, AssetDatabaseItem>();
		foreach (AssetDatabaseItem item in _items)
		{
			_indexedItems[item.Guid] = item;
		}
		return _indexedItems;
	}

	#region IO Manipulation

	public static AssetDatabase Load()
	{
		byte[]? bytes = Engine.Resources.ReadResource(FILE_NAME);
		if (bytes == null)
		{
			return new AssetDatabase();
		}
		
		AssetDatabase? db =   SerializationUtility.DeserializeValue<AssetDatabase>(bytes, FORMAT);
		db ??= new AssetDatabase();
		return db;
	}

	public static AssetDatabase LoadOrCreate(string resourcesPath)
	{
		string path = Path.Join(resourcesPath, FILE_NAME);
		if (!File.Exists(path))
		{
			return new AssetDatabase();
		}
		
		byte[]         json = File.ReadAllBytes(path);
		AssetDatabase? db   =   SerializationUtility.DeserializeValue<AssetDatabase>(json, FORMAT);
		db ??= new AssetDatabase();
		return db;
	}
	
	public void Save(string resourcesPath)
	{
		UpdateListByIndex();
		
		string path = Path.Join(resourcesPath, FILE_NAME);
	
		byte[] json = SerializationUtility.SerializeValue(this, FORMAT);
		File.WriteAllBytes(path, json);

		_dirty = false;
	}
	
	private void UpdateListByIndex()
	{
		if (_indexedItems == null)
		{
			return;
		}

		_items = _indexedItems.Values.ToList();
	}
	#endregion
}