using Key = Silk.NET.Input.Key;

namespace GameToolkit.Framework;

public readonly struct InputKeyboardEvent
{
	public readonly Key  Key;
	public readonly bool Pressed;

	public InputKeyboardEvent(Key key, bool pressed)
	{
		Key     = key;
		Pressed = pressed;
	}
}