using Migration;

namespace Mine.Studio;

[Serializable]
public sealed class DefinitionTemplateField
{
	public DefinitionTemplateFieldType Type;
	public string                      Name;

	public DefinitionTemplateField()
	{
		Type = DefinitionTemplateFieldType.Type_int;
		Name = "newField";
	}

	public DefinitionTemplateField(DefinitionTemplateFieldType type, string name)
	{
		Type = type;
		Name = name;
	}
}

#region Migration
[MigratableInterface(typeof(DefinitionTemplateField))]
public interface IDefinitionTemplateFieldMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionTemplateField))]
public class DefinitionTemplateField_000 : IDefinitionTemplateFieldMigratable
{
	public DefinitionTemplateFieldType Type;
	public string                      Name;
}
#endregion