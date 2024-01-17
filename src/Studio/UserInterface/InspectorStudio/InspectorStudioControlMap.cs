using Mine.Studio;

namespace RedHerring.Studio.UserInterface;

public class InspectorStudioControlMap : InspectorControlMap
{
	public override Type? TypeToControl(Type type)
	{
		if (type.IsSubclassOf(typeof(StudioReference)))
		{
			return typeof(InspectorReferenceControl);
		}

		return base.TypeToControl(type);
	}
}