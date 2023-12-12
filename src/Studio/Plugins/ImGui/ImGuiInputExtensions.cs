using SKey = Silk.NET.Input.Key;
using VKey = Veldrid.Key;
using SMouseButton = Silk.NET.Input.MouseButton;
using VMouseButton = Veldrid.MouseButton;

namespace Mine.ImGuiPlugin;

public static class ImGuiInputExtensions
{
	public static VKey ToVeldridKey(this SKey key)
	{
		return key switch
		{
			SKey.Unknown => VKey.Unknown,
			SKey.Space => VKey.Space,
			SKey.Apostrophe => VKey.Quote,
			SKey.Comma => VKey.Comma,
			SKey.Minus => VKey.Minus,
			SKey.Period => VKey.Period,
			SKey.Slash => VKey.Slash,
			SKey.Number0 => VKey.Number0,
			SKey.Number1 => VKey.Number1,
			SKey.Number2 => VKey.Number2,
			SKey.Number3 => VKey.Number3,
			SKey.Number4 => VKey.Number4,
			SKey.Number5 => VKey.Number5,
			SKey.Number6 => VKey.Number6,
			SKey.Number7 => VKey.Number7,
			SKey.Number8 => VKey.Number8,
			SKey.Number9 => VKey.Number9,
			SKey.Semicolon => VKey.Semicolon,
			SKey.Equal => VKey.Plus,
			SKey.A => VKey.A,
			SKey.B => VKey.B,
			SKey.C => VKey.C,
			SKey.D => VKey.D,
			SKey.E => VKey.E,
			SKey.F => VKey.F,
			SKey.G => VKey.G,
			SKey.H => VKey.H,
			SKey.I => VKey.I,
			SKey.J => VKey.J,
			SKey.K => VKey.K,
			SKey.L => VKey.L,
			SKey.M => VKey.M,
			SKey.N => VKey.N,
			SKey.O => VKey.O,
			SKey.P => VKey.P,
			SKey.Q => VKey.Q,
			SKey.R => VKey.R,
			SKey.S => VKey.S,
			SKey.T => VKey.T,
			SKey.U => VKey.U,
			SKey.V => VKey.V,
			SKey.W => VKey.W,
			SKey.X => VKey.X,
			SKey.Y => VKey.Y,
			SKey.Z => VKey.Z,
			SKey.LeftBracket => VKey.BracketLeft,
			SKey.BackSlash => VKey.BackSlash,
			SKey.RightBracket => VKey.BracketRight,
			SKey.GraveAccent => VKey.Tilde,
			//SKey.World1 => VKey.World1,
			//SKey.World2 => VKey.World2,
			SKey.Escape => VKey.Escape,
			SKey.Enter => VKey.Enter,
			SKey.Tab => VKey.Tab,
			SKey.Backspace => VKey.BackSpace,
			SKey.Insert => VKey.Insert,
			SKey.Delete => VKey.Delete,
			SKey.Right => VKey.Right,
			SKey.Left => VKey.Left,
			SKey.Down => VKey.Down,
			SKey.Up => VKey.Up,
			SKey.PageUp => VKey.PageUp,
			SKey.PageDown => VKey.PageDown,
			SKey.Home => VKey.Home,
			SKey.End => VKey.End,
			SKey.CapsLock => VKey.CapsLock,
			SKey.ScrollLock => VKey.ScrollLock,
			SKey.NumLock => VKey.NumLock,
			SKey.PrintScreen => VKey.PrintScreen,
			SKey.Pause => VKey.Pause,
			SKey.F1 => VKey.F1,
			SKey.F2 => VKey.F2,
			SKey.F3 => VKey.F3,
			SKey.F4 => VKey.F4,
			SKey.F5 => VKey.F5,
			SKey.F6 => VKey.F6,
			SKey.F7 => VKey.F7,
			SKey.F8 => VKey.F8,
			SKey.F9 => VKey.F9,
			SKey.F10 => VKey.F10,
			SKey.F11 => VKey.F11,
			SKey.F12 => VKey.F12,
			SKey.F13 => VKey.F13,
			SKey.F14 => VKey.F14,
			SKey.F15 => VKey.F15,
			SKey.F16 => VKey.F16,
			SKey.F17 => VKey.F17,
			SKey.F18 => VKey.F18,
			SKey.F19 => VKey.F19,
			SKey.F20 => VKey.F20,
			SKey.F21 => VKey.F21,
			SKey.F22 => VKey.F22,
			SKey.F23 => VKey.F23,
			SKey.F24 => VKey.F24,
			SKey.F25 => VKey.F25,
			SKey.Keypad0 => VKey.Keypad0,
			SKey.Keypad1 => VKey.Keypad1,
			SKey.Keypad2 => VKey.Keypad2,
			SKey.Keypad3 => VKey.Keypad3,
			SKey.Keypad4 => VKey.Keypad4,
			SKey.Keypad5 => VKey.Keypad5,
			SKey.Keypad6 => VKey.Keypad6,
			SKey.Keypad7 => VKey.Keypad7,
			SKey.Keypad8 => VKey.Keypad8,
			SKey.Keypad9 => VKey.Keypad9,
			SKey.KeypadDecimal => VKey.KeypadDecimal,
			SKey.KeypadDivide => VKey.KeypadDivide,
			SKey.KeypadMultiply => VKey.KeypadMultiply,
			SKey.KeypadSubtract => VKey.KeypadSubtract,
			SKey.KeypadAdd => VKey.KeypadAdd,
			SKey.KeypadEnter => VKey.KeypadEnter,
			//SKey.KeypadEqual => VKey.KeypadEqual,
			SKey.ShiftLeft => VKey.ShiftLeft,
			SKey.ControlLeft => VKey.ControlLeft,
			SKey.AltLeft => VKey.AltLeft,
			SKey.SuperLeft => VKey.WinLeft,
			SKey.ShiftRight => VKey.ShiftRight,
			SKey.ControlRight => VKey.ControlRight,
			SKey.AltRight => VKey.AltRight,
			SKey.SuperRight => VKey.WinRight,
			SKey.Menu => VKey.Menu,
			_ => VKey.Unknown
		};
	}

	public static VMouseButton ToVeldridMouseButton(this SMouseButton button)
	{
		return button == SMouseButton.Left
			? VMouseButton.Left
			: button == SMouseButton.Right
				? VMouseButton.Right
				: VMouseButton.Middle;
	}

	public static SMouseButton ToSilkMouseButton(this VMouseButton button)
	{
		return button == VMouseButton.Left
			? SMouseButton.Left
			: button == VMouseButton.Right
				? SMouseButton.Right
				: SMouseButton.Middle;
	}
}