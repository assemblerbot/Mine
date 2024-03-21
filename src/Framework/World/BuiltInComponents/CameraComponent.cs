namespace Mine.Framework;

public class CameraComponent : RendererComponent
{
	public CameraComponent(int renderOrder, ulong renderMask, Clipper clipper) : base(renderOrder, renderMask, clipper)
	{
	}

	public override void AfterEntityFlagsChanged(EntityFlags oldFlags, EntityFlags newFlags)
	{
		// TODO - update 
	}
}