namespace SCDearImGui.MonoGame;

/// <summary>
/// Input for types that can indicate to an <see cref="ImGuiRenderer"/> when input has
/// been used by another component, and should thus not be used by the renderer.
/// </summary>
public interface IInputFilter
{
    /// <summary>
    /// Gets a value indicating whether keyboard input has been captured by another component, and should not be used by the GUI renderer.
    /// </summary>
    bool IsKeyboardInputCaptured { get; }

    /// <summary>
    /// Gets a value indicating whether mouse input has been captured by another component, and should not be used by the GUI renderer.
    /// </summary>
    bool IsMouseInputCaptured { get; }
}
