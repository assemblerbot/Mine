namespace Mine.Framework;

public readonly struct InputKeyboardEvent
{
	public readonly Silk.NET.Input.Key Key;
	public readonly bool               Pressed;

	public InputKeyboardEvent(Silk.NET.Input.Key key, bool pressed)
	{
		Key     = key;
		Pressed = pressed;
	}
}