namespace Mine.Framework;

public readonly struct InputMouseEvent
{
	public readonly Silk.NET.Input.MouseButton Button;
	public readonly bool                       Pressed;

	public InputMouseEvent(Silk.NET.Input.MouseButton button, bool pressed)
	{
		Button  = button;
		Pressed = pressed;
	}
}