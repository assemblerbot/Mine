using System.Numerics;
using Mine.Framework;
using Silk.NET.Input;
using Veldrid;
using SMouseButton = Silk.NET.Input.MouseButton;
using VMouseButton = Veldrid.MouseButton;
using SKey = Silk.NET.Input.Key;
using VKey = Veldrid.Key;

namespace Mine.ImGuiPlugin;

public class ImGuiInputSnapshot : InputSnapshot
{
	private readonly List<KeyEvent>   _keyEvents      = new();
	private readonly List<MouseEvent> _mouseEvents    = new();
	private readonly List<char>       _keyCharPresses = new();
	private          Vector2          _mousePosition;
	private          float            _wheelDelta;

	public IReadOnlyList<KeyEvent>   KeyEvents      => _keyEvents;
	public IReadOnlyList<MouseEvent> MouseEvents    => _mouseEvents;
	public IReadOnlyList<char>       KeyCharPresses => _keyCharPresses;
	public Vector2                   MousePosition  => _mousePosition;
	public float                     WheelDelta     => _wheelDelta;

	public bool IsMouseDown(VMouseButton button)
	{
		return Engine.Input.Mouse.IsButtonPressed(button.ToSilkMouseButton());
	}

	public void Update()
	{
		IKeyboard keyboard = Engine.Input.Keyboard;
		IMouse    mouse    = Engine.Input.Mouse;
		
		// key events
		_keyEvents.Clear();
		for (int i = 0; i < Engine.Input.KeyboardEvents.Count; ++i)
		{
			_keyEvents.Add(
				new KeyEvent(
					Engine.Input.KeyboardEvents[i].Key.ToVeldridKey(),
					Engine.Input.KeyboardEvents[i].Pressed,
					(keyboard.IsKeyPressed(SKey.AltLeft) || keyboard.IsKeyPressed(SKey.AltRight) ? ModifierKeys.Alt : ModifierKeys.None)
					|
					(keyboard.IsKeyPressed(SKey.ControlLeft) || keyboard.IsKeyPressed(SKey.ControlRight) ? ModifierKeys.Control : ModifierKeys.None)
					|
					(keyboard.IsKeyPressed(SKey.ShiftLeft) || keyboard.IsKeyPressed(SKey.ShiftRight) ? ModifierKeys.Shift : ModifierKeys.None),
					false
				)
			);
		}

		// mouse events
		_mouseEvents.Clear();
		for(int i=0;i<Engine.Input.MouseEvents.Count;i++)
		{
			_mouseEvents.Add(
				new MouseEvent(
					Engine.Input.MouseEvents[i].Button.ToVeldridMouseButton(),
					Engine.Input.MouseEvents[i].Pressed
				)
			);
		}
		
		// key char presses
		_keyCharPresses.Clear();
		_keyCharPresses.AddRange(Engine.Input.KeyboardCharEvents);
		
		// mouse position
		_mousePosition = mouse.Position;

		// wheel delta
		if (mouse.ScrollWheels.Count > 0)
		{
			_wheelDelta = mouse.ScrollWheels[0].Y;
		}
	}
}