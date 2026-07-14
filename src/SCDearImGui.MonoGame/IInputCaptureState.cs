namespace SCDearImGui.MonoGame;

/// <summary>
/// <para>
/// Interface for types that store indications of whether mouse or keyboard input has been captured
/// by some component, and should therefore not be used by anything else.
/// </para>
/// <para>
/// An implementation of this type can optionally be passed to <see cref="ImGuiRenderer"/>, to keep it
/// from consuming input when appropriate, and keep track of when it has captured input.
/// </para>
/// <para>
/// NB: Yes, it doesn't make a huge amount of sense for consumers to set 'Is..Captured' props to false, 
/// thus 'uncapturing' inputs. But of course ImGuiRenderer will never actually do this, and this design 
/// (as opposed to a design involving a gettable property and a 'Capture..' method) means a minimal
/// implementation is completely trivial, which is nice. More sophisticated implementations can of
/// course simply throw if anything attempts to 'uncapture'.
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
