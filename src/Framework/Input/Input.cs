using Silk.NET.Input;
using Silk.NET.Windowing;

namespace GameToolkit.Framework;

public sealed class Input : IDisposable
{
	private readonly IInputContext _inputContext;

	private IKeyboard? _keyboard;
	private IMouse?    _mouse;
	private IGamepad?  _gamepad;
	
	public Input(IView view)
	{
		_inputContext = view.CreateInput();
		_inputContext.ConnectionChanged += OnConnectionChanged;

		_keyboard = FirstKeyboard();
		_mouse    = FirstMouse();
		_gamepad  = FirstGamepad();

		//_keyboard.IsKeyPressed(Key.A);
	}

	public void Dispose()
	{
		_inputContext.ConnectionChanged -= OnConnectionChanged;
		_inputContext.Dispose();
	}

	private void OnConnectionChanged(IInputDevice device, bool isConnected)
	{
		switch (device)
		{
			case IKeyboard keyboard:
				_keyboard = isConnected ? keyboard : FirstKeyboard();
				return;
			case IMouse mouse:
				_mouse = isConnected ? mouse : FirstMouse();
				return;
			case IGamepad gamepad:
				_gamepad = isConnected ? gamepad : FirstGamepad();
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
}