using System;
using System.Collections.Generic;
using SlimDXKey = SlimDXKeys.Key;

namespace FDK
{
	public class DeviceConstantConverter
	{
		/// <returns>
		///		対応する値がなければ SlimDX.DirectInput.Unknown を返す。
		/// </returns>
		public static SlimDXKey TKKtoKey(OpenTK.Input.Key key)
		{
			if (_TKKtoKey.ContainsKey(key))
			{
				return _TKKtoKey[key];
			}
			else
			{
				return SlimDXKey.Unknown;
			}
		}

		/// <summary>
		///		DIK (SharpDX.DirectInput.Key) から SlimDX.DirectInput.Key への変換表。
		/// </summary>
		private static readonly Dictionary<OpenTK.Input.Key, SlimDXKey> _TKKtoKey = new Dictionary<OpenTK.Input.Key, SlimDXKey>() {
			#region [ *** ]
			{ OpenTK.Input.Key.Unknown, SlimDXKey.Unknown },
			{ OpenTK.Input.Key.Escape, SlimDXKey.Escape },
			{ OpenTK.Input.Key.Number1, SlimDXKey.D1 },
			{ OpenTK.Input.Key.Number2, SlimDXKey.D2 },
			{ OpenTK.Input.Key.Number3, SlimDXKey.D3 },
			{ OpenTK.Input.Key.Number4, SlimDXKey.D4 },
			{ OpenTK.Input.Key.Number5, SlimDXKey.D5 },
			{ OpenTK.Input.Key.Number6, SlimDXKey.D6 },
			{ OpenTK.Input.Key.Number7, SlimDXKey.D7 },
			{ OpenTK.Input.Key.Number8, SlimDXKey.D8 },
			{ OpenTK.Input.Key.Number9, SlimDXKey.D9 },
			{ OpenTK.Input.Key.Number0, SlimDXKey.D0 },
			{ OpenTK.Input.Key.Minus, SlimDXKey.Minus },
			{ OpenTK.Input.Key.BackSpace, SlimDXKey.Backspace },
			{ OpenTK.Input.Key.Tab, SlimDXKey.Tab },
			{ OpenTK.Input.Key.Q, SlimDXKey.Q },
			{ OpenTK.Input.Key.W, SlimDXKey.W },
			{ OpenTK.Input.Key.E, SlimDXKey.E },
			{ OpenTK.Input.Key.R, SlimDXKey.R },
			{ OpenTK.Input.Key.T, SlimDXKey.T },
			{ OpenTK.Input.Key.Y, SlimDXKey.Y },
			{ OpenTK.Input.Key.U, SlimDXKey.U },
			{ OpenTK.Input.Key.I, SlimDXKey.I },
			{ OpenTK.Input.Key.O, SlimDXKey.O },
			{ OpenTK.Input.Key.P, SlimDXKey.P },
			{ OpenTK.Input.Key.BracketLeft, SlimDXKey.LeftBracket },
			{ OpenTK.Input.Key.BracketRight, SlimDXKey.RightBracket },
			{ OpenTK.Input.Key.Enter, SlimDXKey.Return },
			{ OpenTK.Input.Key.ControlLeft, SlimDXKey.LeftControl },
			{ OpenTK.Input.Key.A, SlimDXKey.A },
			{ OpenTK.Input.Key.S, SlimDXKey.S },
			{ OpenTK.Input.Key.D, SlimDXKey.D },
			{ OpenTK.Input.Key.F, SlimDXKey.F },
			{ OpenTK.Input.Key.G, SlimDXKey.G },
			{ OpenTK.Input.Key.H, SlimDXKey.H },
			{ OpenTK.Input.Key.J, SlimDXKey.J },
			{ OpenTK.Input.Key.K, SlimDXKey.K },
			{ OpenTK.Input.Key.L, SlimDXKey.L },
			{ OpenTK.Input.Key.Semicolon, SlimDXKey.Semicolon },
			{ OpenTK.Input.Key.Quote, SlimDXKey.Apostrophe },
			{ OpenTK.Input.Key.Grave, SlimDXKey.Grave },
			{ OpenTK.Input.Key.ShiftLeft, SlimDXKey.LeftShift },
			{ OpenTK.Input.Key.BackSlash, SlimDXKey.Backslash },
			{ OpenTK.Input.Key.NonUSBackSlash, SlimDXKey.Yen },
			{ OpenTK.Input.Key.Z, SlimDXKey.Z },
			{ OpenTK.Input.Key.X, SlimDXKey.X },
			{ OpenTK.Input.Key.C, SlimDXKey.C },
			{ OpenTK.Input.Key.V, SlimDXKey.V },
			{ OpenTK.Input.Key.B, SlimDXKey.B },
			{ OpenTK.Input.Key.N, SlimDXKey.N },
			{ OpenTK.Input.Key.M, SlimDXKey.M },
			{ OpenTK.Input.Key.Comma, SlimDXKey.Comma },
			{ OpenTK.Input.Key.Period, SlimDXKey.Period },
			{ OpenTK.Input.Key.Slash, SlimDXKey.Slash },
			{ OpenTK.Input.Key.ShiftRight, SlimDXKey.RightShift },
			{ OpenTK.Input.Key.KeypadMultiply, SlimDXKey.NumberPadStar },
			{ OpenTK.Input.Key.AltLeft, SlimDXKey.LeftAlt },
			{ OpenTK.Input.Key.Space, SlimDXKey.Space },
			{ OpenTK.Input.Key.CapsLock, SlimDXKey.CapsLock },
			{ OpenTK.Input.Key.F1, SlimDXKey.F1 },
			{ OpenTK.Input.Key.F2, SlimDXKey.F2 },
			{ OpenTK.Input.Key.F3, SlimDXKey.F3 },
			{ OpenTK.Input.Key.F4, SlimDXKey.F4 },
			{ OpenTK.Input.Key.F5, SlimDXKey.F5 },
			{ OpenTK.Input.Key.F6, SlimDXKey.F6 },
			{ OpenTK.Input.Key.F7, SlimDXKey.F7 },
			{ OpenTK.Input.Key.F8, SlimDXKey.F8 },
			{ OpenTK.Input.Key.F9, SlimDXKey.F9 },
			{ OpenTK.Input.Key.F10, SlimDXKey.F10 },
			{ OpenTK.Input.Key.NumLock, SlimDXKey.NumberLock },
			{ OpenTK.Input.Key.ScrollLock, SlimDXKey.ScrollLock },
			{ OpenTK.Input.Key.Keypad7, SlimDXKey.NumberPad7 },
			{ OpenTK.Input.Key.Keypad8, SlimDXKey.NumberPad8 },
			{ OpenTK.Input.Key.Keypad9, SlimDXKey.NumberPad9 },
			{ OpenTK.Input.Key.KeypadSubtract, SlimDXKey.NumberPadMinus },
			{ OpenTK.Input.Key.Keypad4, SlimDXKey.NumberPad4 },
			{ OpenTK.Input.Key.Keypad5, SlimDXKey.NumberPad5 },
			{ OpenTK.Input.Key.Keypad6, SlimDXKey.NumberPad6 },
			{ OpenTK.Input.Key.KeypadAdd, SlimDXKey.NumberPadPlus },
			{ OpenTK.Input.Key.Keypad1, SlimDXKey.NumberPad1 },
			{ OpenTK.Input.Key.Keypad2, SlimDXKey.NumberPad2 },
			{ OpenTK.Input.Key.Keypad3, SlimDXKey.NumberPad3 },
			{ OpenTK.Input.Key.Keypad0, SlimDXKey.NumberPad0 },
			{ OpenTK.Input.Key.KeypadDecimal, SlimDXKey.NumberPadPeriod },
			{ OpenTK.Input.Key.F11, SlimDXKey.F11 },
			{ OpenTK.Input.Key.F12, SlimDXKey.F12 },
			{ OpenTK.Input.Key.F13, SlimDXKey.F13 },
			{ OpenTK.Input.Key.F14, SlimDXKey.F14 },
			{ OpenTK.Input.Key.F15, SlimDXKey.F15 },
			{ OpenTK.Input.Key.KeypadEnter, SlimDXKey.NumberPadEnter },
			{ OpenTK.Input.Key.ControlRight, SlimDXKey.RightControl },
			{ OpenTK.Input.Key.PrintScreen, SlimDXKey.PrintScreen },
			{ OpenTK.Input.Key.AltRight, SlimDXKey.RightAlt },
			{ OpenTK.Input.Key.Pause, SlimDXKey.Pause },
			{ OpenTK.Input.Key.Home, SlimDXKey.Home },
			{ OpenTK.Input.Key.Up, SlimDXKey.UpArrow },
			{ OpenTK.Input.Key.PageUp, SlimDXKey.PageUp },
			{ OpenTK.Input.Key.Left, SlimDXKey.LeftArrow },
			{ OpenTK.Input.Key.Right, SlimDXKey.RightArrow },
			{ OpenTK.Input.Key.End, SlimDXKey.End },
			{ OpenTK.Input.Key.Down, SlimDXKey.DownArrow },
			{ OpenTK.Input.Key.PageDown, SlimDXKey.PageDown },
			{ OpenTK.Input.Key.Insert, SlimDXKey.Insert },
			{ OpenTK.Input.Key.Delete, SlimDXKey.Delete },
			{ OpenTK.Input.Key.WinLeft, SlimDXKey.LeftWindowsKey },
			{ OpenTK.Input.Key.WinRight, SlimDXKey.RightWindowsKey },
			{ OpenTK.Input.Key.Sleep, SlimDXKey.Sleep },
			#endregion
		};
	}
}
