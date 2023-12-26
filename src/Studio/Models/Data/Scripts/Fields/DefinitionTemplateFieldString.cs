namespace Mine.Studio;

public sealed class DefinitionTemplateFieldString : DefinitionTemplateField
{
	public override string TypeName => "string";

	public DefinitionTemplateFieldString() : base("new")
	{
	}
	
	public DefinitionTemplateFieldString(string name) : base(name)
	{
	}
}