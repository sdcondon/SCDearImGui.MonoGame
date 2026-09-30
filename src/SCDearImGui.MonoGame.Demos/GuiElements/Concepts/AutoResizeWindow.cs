using ImGuiNET;
using static ImGuiNET.ImGui;
using static SCDearImGui.MonoGame.Demos.GuiElements.GuiElementHelpers;

namespace SCDearImGui.MonoGame.Demos.GuiElements.Concepts;

// A window which gets auto-resized according to its content.
class AutoResizeWindow(bool isOpen = false)
{
    // Yes, public fields is unusual/bad, but this very succinctly allows hooking up the close button with a
    // pass-by-reference in the Begin call (see below). Obviously wouldn't take much work to do this without
    // needing a public field, but not bothering to keep the demo lean and mean.
    public bool IsOpen = isOpen;

    private int lineCount = 10;

    public void Update()
    {
        if (!IsOpen) return;

        if (Begin("Concept Demo: Automatic Resizing", ref IsOpen, ImGuiWindowFlags.AlwaysAutoResize))
        {
            TextUnformatted(
                "This window will resize to the size of its content on every frame."
                + "\nNote that you probably don't want to query the window size to"
                + "\noutput your content because that would create a feedback loop.");

            SliderInt("Number of lines", ref lineCount, 1, 20);

            for (int i = 0; i < lineCount; i++)
            {
                // NB: Pad with space to extend size horizontally
                var padding = new string(' ', i * 4);
                Text($"{padding}This is line {i}");
            }
        }

        End();
    }
}
