using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Mine.Framework;

public sealed class Input : IDisposable
{
	private readonly IInputContext _inputContext;

	private IKeyboard? _keyboard;
	private IMouse?    _mouse;
	private IGamepad?  _gamepad;
	
	public IKeyboard Keyboard => _keyboard ?? throw new NullReferenceException("No keyboard connected");
	public IMouse    Mouse    => _mouse    ?? throw new NullReferenceException("No mouse connected");
	public IGamepad  Gamepad  => _gamepad  ?? throw new NullReferenceException("No gamepad connected");

	public readonly List<char>               KeyboardCharEvents = new();
	public readonly List<InputKeyboardEvent> KeyboardEvents     = new();
	public readonly List<InputMouseEvent>    MouseEvents        = new();
	public readonly List<Button>             GamepadButtons     = new();
	
	public Input(IView view)
	{
		_inputContext                   =  view.CreateInput();
		_inputContext.ConnectionChanged += OnConnectionChanged;

		_keyboard = FirstKeyboard();
		_mouse    = FirstMouse();
		_gamepad  = FirstGamepad();

		RegisterKeyboardCallbacks(_keyboard);
		RegisterMouseCallbacks(_mouse);
		RegisterGamepadCallbacks(_gamepad);
	}

	public void EndOfFrame()
	{
		ResetKeyboardEvents();
		ResetMouseEvents();
		ResetGamepadEvents();
	}

	public void Dispose()
	{
		UnregisterKeyboardCallbacks(_keyboard);
		UnregisterMouseCallbacks(_mouse);
		UnregisterGamepadCallbacks(_gamepad);
		
		_inputContext.ConnectionChanged -= OnConnectionChanged;
		_inputContext.Dispose();
	}
	
	#region Input device management
	private void OnConnectionChanged(IInputDevice device, bool isConnected)
	{
		switch (device)
		{
			case IKeyboard keyboard:
				UnregisterKeyboardCallbacks(_keyboard);
				_keyboard = isConnected ? keyboard : FirstKeyboard();
				RegisterKeyboardCallbacks(_keyboard);
				return;
			case IMouse mouse:
				UnregisterMouseCallbacks(_mouse);
				_mouse = isConnected ? mouse : FirstMouse();
				RegisterMouseCallbacks(_mouse);
				return;
			case IGamepad gamepad:
				UnregisterGamepadCallbacks(_gamepad);
				_gamepad = isConnected ? gamepad : FirstGamepad();
				RegisterGamepadCallbacks(_gamepad);
				return;
		}
	}
	
	private IKeyboard? FirstKeyboard()
	{
		return _inputContext.Keyboards.FirstOrDefault();
	}

	private IMouse? FirstMouse()
	{
		return _inputContext.Mice.FirstOrDefault();
	}

	private IGamepad? FirstGamepad()
	{
		return _inputContext.Gamepads.FirstOrDefault();
	}
	#endregion
	
	#region Keyboard callbacks
	private void RegisterKeyboardCallbacks(IKeyboard? keyboard)
	{
		if (keyboard == null)
		{
			return;
		}

		keyboard.KeyChar += OnKeyboardChar;
		keyboard.KeyDown += OnKeyboardKeyDown;
		keyboard.KeyUp   += OnKeyboardKeyUp;
	}

	private void UnregisterKeyboardCallbacks(IKeyboard? keyboard)
	{
		if (keyboard == null)
		{
			return;
		}

		keyboard.KeyChar -= OnKeyboardChar;
		keyboard.KeyDown -= OnKeyboardKeyDown;
		keyboard.KeyUp   -= OnKeyboardKeyUp;
	}

	private void ResetKeyboardEvents()
	{
		KeyboardCharEvents.Clear();
		KeyboardEvents.Clear();
	}

	private void OnKeyboardChar(IKeyboard keyboard, char character)
	{
		KeyboardCharEvents.Add(character);
	}

	private void OnKeyboardKeyDown(IKeyboard keyboard, Key key, int value)
	{
		KeyboardEvents.Add(new InputKeyboardEvent(key, true));
	}

	private void OnKeyboardKeyUp(IKeyboard keyboard, Key key, int value)
	{
		KeyboardEvents.Add(new InputKeyboardEvent(key, false));
	}
	#endregion
	
	#region Mouse callbacks
	private void RegisterMouseCallbacks(IMouse? mouse)
	{
		if (mouse == null)
		{
			return;
		}

		mouse.MouseDown += OnMouseDown;
		mouse.MouseUp   += OnMouseUp;
	}

	private void UnregisterMouseCallbacks(IMouse? mouse)
	{
		if (mouse == null)
		{
			return;
		}

		mouse.MouseDown -= OnMouseDown;
		mouse.MouseUp   -= OnMouseUp;
	}
	
	private void ResetMouseEvents()
	{
		MouseEvents.Clear();
	}

	private void OnMouseDown(IMouse mouse, MouseButton button)
	{
		MouseEvents.Add(new InputMouseEvent(button, true));
	}

	private void OnMouseUp(IMouse mouse, MouseButton button)
	{
		MouseEvents.Add(new InputMouseEvent(button, false));
	}
	#endregion
	
	#region Gamepad callbacks
	private void RegisterGamepadCallbacks(IGamepad? gamepad)
	{
		if (gamepad == null)
		{
			return;
		}
		
		gamepad.ButtonDown      += OnGamepadButtonDown;
		gamepad.ButtonUp        += OnGamepadButtonUp;
		gamepad.TriggerMoved    += OnGamepadTriggerMoved;
		gamepad.ThumbstickMoved += OnGamepadThumbstickMoved;
	}

	private void UnregisterGamepadCallbacks(IGamepad? gamepad)
	{
		if (gamepad == null)
		{
			return;
		}
		
		gamepad.ButtonDown      -= OnGamepadButtonDown;
		gamepad.ButtonUp        -= OnGamepadButtonUp;
		gamepad.TriggerMoved    -= OnGamepadTriggerMoved;
		gamepad.ThumbstickMoved -= OnGamepadThumbstickMoved;
	}
	
	private void ResetGamepadEvents()
	{
		GamepadButtons.Clear();
	}
	
	private void OnGamepadButtonDown(IGamepad gamepad, Button button)
	{
		GamepadButtons.Add(button);
	}

	private void OnGamepadButtonUp(IGamepad gamepad, Button button)
	{
		GamepadButtons.Add(button);
	}
	
	private void OnGamepadTriggerMoved(IGamepad gamepad, Trigger trigger)
	{
		// is that useful?
	}

	private void OnGamepadThumbstickMoved(IGamepad gamepad, Thumbstick thumbstick)
	{
		// is that useful?
	}
	#endregion
}