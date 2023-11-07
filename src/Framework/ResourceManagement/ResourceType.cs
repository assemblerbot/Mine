namespace GameToolkit.Framework;

public enum ResourceType
{
	Unknown, // unknown resource type
	
	Binary, // binary data
	Text,   // text data
	Json,   // json
	Prefab, // json, prefab hierarchy 
	Mesh,   // json, mesh
	Image,  // json, image
	Sound,  // json sound?
}

public static class ResourceTypeExtensions
{
	public static ResourceType FileExtensionToResourceType(string fileExtension)
	{
		string lowercaseFileExtension = fileExtension.ToLower();
		
		switch(fileExtension)
		{
			case "bin":    return ResourceType.Binary;
			case "txt":    return ResourceType.Text;
			case "json":   return ResourceType.Json;
			case "prefab": return ResourceType.Prefab;

			case "fbx":
			case "obj":
			case "blender":
				return ResourceType.Mesh;
			
			default:
				return ResourceType.Unknown;
		}
	}
}