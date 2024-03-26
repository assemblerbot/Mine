namespace Mine.Framework;

//------------------------------------------------------------------------------------
// non generic base class
public abstract class LightComponent : Component, IRenderable
{
	private ulong _renderMask;
	public  ulong RenderMask => _renderMask;

	protected LightComponent(ulong renderMask)
	{
		_renderMask = renderMask;
	}

	public override void AfterAddedToWorld()
	{
		Engine.World.RegisterLight(this);
	}

	public override void BeforeRemovedFromWorld()
	{
		Engine.World.UnregisterLight(this);
	}
}

//------------------------------------------------------------------------------------
// generic base class
public abstract class LightComponent<TResourceSet> : LightComponent
	where TResourceSet : ShaderResourceSet, new()
{
	//private readonly ShaderResourceSet _resourceSet = new TResourceSet();

	protected LightComponent(ulong renderMask) : base(renderMask)
	{
	}

	public override void Dispose()
	{
		//_resourceSet.Dispose();
		base.Dispose();
	}
}