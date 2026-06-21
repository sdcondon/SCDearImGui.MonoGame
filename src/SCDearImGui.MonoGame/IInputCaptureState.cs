namespace SCDearImGui.MonoGame;

/// <summary>
/// <para>
/// Interface for types that store indications of whether mouse or keyboard input has been captured
/// by some component, and should not therefore be used by anything else.
/// </para>
/// <para>
/// An implementation of this type can optioanlly be passed to <see cref="ImGuiRenderer"/>, to keep it
/// from consuming input when appropriate, and keep track of when it has captured input.
/// </para>
/// </summary>
public interface IInputCaptureState
{
    /// <summary>
    /// Gets or sets a value indicating whether keyboard input has been captured, and should not be used by any other component.
    /// </summary>
    bool IsKeyboardCaptured { get; set; }

    /// <summary>
    /// Gets a value indicating whether mouse input has been captured, and should not be used by any other component.
    /// </summary>
    bool IsMouseCaptured { get; set; }
}
