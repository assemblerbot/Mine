namespace Mine.Studio;

[Serializable]
public enum DefinitionTemplateFieldType
{
	Type_int    = 0,
	Type_float  = 1,
	Type_string = 2,
	Type_bool   = 3,
	
	Type_AssetReference = 100,
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

	public static DefinitionAssetValue ToDefinitionAssetValue(this DefinitionTemplateFieldType type)
	{
		return type switch
		{
			DefinitionTemplateFieldType.Type_int            => new DefinitionAssetValueInt(),
			DefinitionTemplateFieldType.Type_float          => new DefinitionAssetValueFloat(),
			DefinitionTemplateFieldType.Type_string         => new DefinitionAssetValueString(),
			DefinitionTemplateFieldType.Type_bool           => new DefinitionAssetValueBool(),
			DefinitionTemplateFieldType.Type_AssetReference => new DefinitionAssetValueAssetReference(),
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}
}