using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("3b2b36d4-9272-490b-87e4-495393682604")]
public sealed class DefinitionTemplateField
{
	public DefinitionTemplateFieldType Type;
	public string                      Name;
	public string                      GenericParameter; // TODO - make a dropdown list by Type

	public DefinitionTemplateField()
	{
		Type             = DefinitionTemplateFieldType.Type_int;
		Name             = "newField";
		GenericParameter = "";
	}

	public DefinitionTemplateField(DefinitionTemplateFieldType type, string name, string genericParameter = "")
	{
		Type             = type;
		Name             = name;
		GenericParameter = genericParameter;
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
	public string                      GenericParameter;
}
#endregion