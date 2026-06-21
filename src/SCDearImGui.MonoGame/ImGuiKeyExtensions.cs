using ImGuiNET;
using Microsoft.Xna.Framework.Input;

namespace SCDearImGui.MonoGame;

/// <summary>
/// Extension methods for converting between <see cref="ImGuiKey"/> and <see cref="Keys"/>.
/// </summary>
public static class ImGuiKeyExtensions
{
    /// <summary>
    /// Attempts to convert a <see cref="Keys"/> value to its <see cref="ImGuiKey"/> equivalent.
    /// </summary>
    /// <param name="key">The key value to convert.</param>
    /// <returns>The <see cref="ImGuiKey"/> equivalent of the this key - or <see langword="null"/> if an equivalent does not exist.</returns>
    public static ImGuiKey? TryToImGuiKey(this Keys key)
    {
        return key switch
        {
            Keys.None => ImGuiKey.None,
            Keys.Back => ImGuiKey.Backspace,
            Keys.Tab => ImGuiKey.Tab,
            Keys.Enter => ImGuiKey.Enter,
            Keys.CapsLock => ImGuiKey.CapsLock,
            Keys.Escape => ImGuiKey.Escape,
            Keys.Space => ImGuiKey.Space,
            Keys.PageUp => ImGuiKey.PageUp,
            Keys.PageDown => ImGuiKey.PageDown,
            Keys.End => ImGuiKey.End,
            Keys.Home => ImGuiKey.Home,
            Keys.Left => ImGuiKey.LeftArrow,
            Keys.Right => ImGuiKey.RightArrow,
            Keys.Up => ImGuiKey.UpArrow,
            Keys.Down => ImGuiKey.DownArrow,
            Keys.PrintScreen => ImGuiKey.PrintScreen,
            Keys.Insert => ImGuiKey.Insert,
            Keys.Delete => ImGuiKey.Delete,
            >= Keys.D0 and <= Keys.D9 => ImGuiKey._0 + (key - Keys.D0),
            >= Keys.A and <= Keys.Z => ImGuiKey.A + (key - Keys.A),
            >= Keys.NumPad0 and <= Keys.NumPad9 => ImGuiKey.Keypad0 + (key - Keys.NumPad0),
            Keys.Multiply => ImGuiKey.KeypadMultiply,
            Keys.Add => ImGuiKey.KeypadAdd,
            Keys.Subtract => ImGuiKey.KeypadSubtract,
            Keys.Decimal => ImGuiKey.KeypadDecimal,
            Keys.Divide => ImGuiKey.KeypadDivide,
            >= Keys.F1 and <= Keys.F12 => ImGuiKey.F1 + (key - Keys.F1),
            Keys.NumLock => ImGuiKey.NumLock,
            Keys.Scroll => ImGuiKey.ScrollLock,
            Keys.LeftShift => ImGuiKey.ModShift,
            Keys.LeftControl => ImGuiKey.ModCtrl,
            Keys.LeftAlt => ImGuiKey.ModAlt,
            Keys.OemSemicolon => ImGuiKey.Semicolon,
            Keys.OemPlus => ImGuiKey.Equal,
            Keys.OemComma => ImGuiKey.Comma,
            Keys.OemMinus => ImGuiKey.Minus,
            Keys.OemPeriod => ImGuiKey.Period,
            Keys.OemQuestion => ImGuiKey.Slash,
            Keys.OemTilde => ImGuiKey.GraveAccent,
            Keys.OemOpenBrackets => ImGuiKey.LeftBracket,
            Keys.OemCloseBrackets => ImGuiKey.RightBracket,
            Keys.OemPipe => ImGuiKey.Backslash,
            Keys.OemQuotes => ImGuiKey.Apostrophe,
            _ => null,
        };
    }

    /// <summary>
    /// Attempts to convert a <see cref="Keys"/> value to its <see cref="ImGuiKey"/> equivalent.
    /// </summary>
    /// <param name="key">The key value to convert.</param>
    /// <returns>The <see cref="ImGuiKey"/> equivalent of this key - or <see cref="ImGuiKey.None"/> if an equivalent does not exist.</returns>
    public static ImGuiKey ToImGuiKey(this Keys key)
    {
        return TryToImGuiKey(key) ?? ImGuiKey.None;
    }
}
