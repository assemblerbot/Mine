using Migration;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable]
public abstract class DefinitionTemplateField
{
	public abstract string TypeName { get; }
	public string Name;

	protected DefinitionTemplateField(string name)
	{
		Name = name;
	}

	public static DefinitionTemplateField? CreateFromType(string typeName, string name)
	{
		switch (typeName)
		{
			case "int" :    return new DefinitionTemplateFieldInt(name);
			case "float" :  return new DefinitionTemplateFieldFloat(name);
			case "string" : return new DefinitionTemplateFieldString(name);
			case "bool" :   return new DefinitionTemplateFiledBool(name);
		}

		return null;
	}
}

#region Migration
[MigratableInterface(typeof(DefinitionTemplateField))]
public interface IDefinitionTemplateFieldMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionTemplateField))]
public class DefinitionTemplateField_000 : IDefinitionTemplateFieldMigratable
{
	public string Name;
}
#endregion