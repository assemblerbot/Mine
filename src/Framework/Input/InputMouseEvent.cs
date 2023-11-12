using Button = Silk.NET.Input.MouseButton;

namespace Mine.Framework;

public readonly struct InputMouseEvent
{
	public readonly Button Button;
	public readonly bool   Pressed;

	public InputMouseEvent(Button button, bool pressed)
	{
		Button  = button;
		Pressed = pressed;
	}
}