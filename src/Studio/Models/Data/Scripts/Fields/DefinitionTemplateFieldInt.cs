namespace Mine.Studio;

public sealed class DefinitionTemplateFieldInt : DefinitionTemplateField
{
	public override string TypeName => "int";

	public DefinitionTemplateFieldInt() : base("new")
	{
	}
	
	public DefinitionTemplateFieldInt(string name) : base(name)
	{
	}
}