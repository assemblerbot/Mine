using RedHerring.Studio.Models.Project;

namespace Mine.Studio;

// !!! Don't rename without thinking! Enum values are used as data types for definition fields! !!!
[Serializable]
public enum DefinitionTemplateFieldType
{
	Type_int    = 0,
	Type_float  = 1,
	Type_string = 2,
	Type_bool   = 3,
	
	Type_AssetReference      = 100,
	Type_FolderReference     = 101,
	Type_DefinitionReference = 102,
}

public static class DefinitionTemplateFiledTypeExtensions
{
	public static string ToCSharpType(this DefinitionTemplateFieldType type)
	{
		return type.ToString().Substring("Type_".Length);
	}

	public static DefinitionTemplateFieldType ToTemplateType(this string csharpType)
	{
		return Enum.TryParse(typeof(DefinitionTemplateFieldType), "Type_" + csharpType, out object? result)
			? (DefinitionTemplateFieldType) result
			: DefinitionTemplateFieldType.Type_int;
	}

	public static DefinitionAssetValue ToDefinitionAssetValue(this DefinitionTemplateFieldType type, StudioScriptDefinitionReference genericParameter)
	{
		return type switch
		{
			DefinitionTemplateFieldType.Type_int            => new DefinitionAssetValueInt(),
			DefinitionTemplateFieldType.Type_float          => new DefinitionAssetValueFloat(),
			DefinitionTemplateFieldType.Type_string         => new DefinitionAssetValueString(),
			DefinitionTemplateFieldType.Type_bool           => new DefinitionAssetValueBool(),
			DefinitionTemplateFieldType.Type_AssetReference => new DefinitionAssetValueReference(new StudioAssetReference()),
			DefinitionTemplateFieldType.Type_FolderReference => new DefinitionAssetValueReference(new StudioFolderReference()),
			DefinitionTemplateFieldType.Type_DefinitionReference => new DefinitionAssetValueReference(new StudioAssetDefinitionReference(genericParameter.Guid ?? "")),
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}
	
	public static bool HasGenericParameter(this DefinitionTemplateFieldType type)
	{
		return type == DefinitionTemplateFieldType.Type_DefinitionReference;
	}

	public static bool IsReference(this DefinitionTemplateFieldType type)
	{
		return
			type == DefinitionTemplateFieldType.Type_AssetReference  ||
			type == DefinitionTemplateFieldType.Type_FolderReference ||
			type == DefinitionTemplateFieldType.Type_DefinitionReference;
	}
}