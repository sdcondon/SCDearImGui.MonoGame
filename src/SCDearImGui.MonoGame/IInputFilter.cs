namespace SCDearImGui.MonoGame;

/// <summary>
/// Input for types that can filter the input passed to the ImGuiRenderer.
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
