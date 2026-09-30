using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static ImGuiNET.ImGui;
using static SCDearImGui.MonoGame.Demos.GuiElements.GuiElementHelpers;

namespace SCDearImGui.MonoGame.Demos.GuiElements.Concepts;

// Demonstrate creating a window with custom resize constraints.
// Note that size constraints currently don't work on a docked window (when in 'docking' branch)
// https://github.com/ocornut/imgui/blob/master/imgui_demo.cpp#L9601
class ConstrainedResizeWindow(bool isOpen = false)
{
    public bool IsOpen = isOpen;

    // NB: for the custom ones, we dont use UserData because callback signature isn't right to do
    // it easily. Instead, we just keep the data on the managed side via a compiler-generated closure:
    private static readonly unsafe List<SizeConstraintOption> sizeConstraintOptions =
    [
        new("Between 100x100 and 500x500", new(100, 100), new(500, 500), null),
        new("At least 100x100", new(100, 100), new(float.MaxValue, float.MaxValue), null),
        new("Resize vertical + lock current width", new(-1, 0), new(-1, float.MaxValue), null),
        new("Resize horizontal + lock current height", new(0, -1), new(float.MaxValue, -1), null),
        new("Width Between 400 and 500", new(400, -1), new(500, -1), null),
        new("Height at least 400", new(-1, 400), new(-1, float.MaxValue), null),
        new("Custom: Aspect Ratio 16:9", new(0, 0), new(float.MaxValue, float.MaxValue), p => AspectRatioSizeCallback(16f/9f, p)),
        new("Custom: Always Square", new(0, 0), new(float.MaxValue, float.MaxValue), SquareSizeCallback),
        new("Custom: Fixed Steps (100)", new(0, 0), new(float.MaxValue, float.MaxValue), p => StepSizeCallback(100, p)),
    ];

    private bool auto_resize = false;
    private bool window_padding = true;
    private int sizeConstraintOptionIndex = 0;
    private int display_lines = 10;

    public void Update()
    {
        if (!IsOpen) return;

        // Submit window
        if (!window_padding) PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        ApplySizeConstraintOption(sizeConstraintOptions[sizeConstraintOptionIndex]);

        bool window_open = Begin(
            "Example: Constrained Resize",
            ref IsOpen,
            auto_resize ? ImGuiWindowFlags.AlwaysAutoResize : 0);

        if (!window_padding) PopStyleVar();

        if (window_open)
        {
            if (GetIO().KeyShift)
            {
                // Display a dummy viewport (in your real app you would likely use ImageButton() to display a texture)
                var avail_size = GetContentRegionAvail();
                var pos = GetCursorScreenPos();
                ColorButton("viewport", new(0.5f, 0.2f, 0.5f, 1.0f), ImGuiColorEditFlags.NoTooltip | ImGuiColorEditFlags.NoDragDrop, avail_size);
                SetCursorScreenPos(new(pos.X + 10, pos.Y + 10));
                Text($"{avail_size.X:F2} x {avail_size.Y:F2}");
            }
            else
            {
                Text("(Hold SHIFT to display a dummy viewport)");
                if (Button("Set 200x200")) SetWindowSize(new(200, 200));
                SameLine();
                if (Button("Set 500x500")) SetWindowSize(new(500, 500));
                SameLine();
                if (Button("Set 800x200")) SetWindowSize(new(800, 200));
                SetNextItemWidth(GetFontSize() * 20);

                Combo(
                    "Constraint",
                    ref sizeConstraintOptionIndex,
                    string.Join("\0", sizeConstraintOptions.Select(o => o.Description)),
                    sizeConstraintOptions.Count);
                
                SetNextItemWidth(GetFontSize() * 20);
                DragInt("Lines", ref display_lines, 0.2f, 1, 100);
                Checkbox("Auto-resize", ref auto_resize);
                Checkbox("Window padding", ref window_padding);

                for (int i = 0; i < display_lines; i++)
                    Text($"{new string(' ', i * 4)}Hello, sailor! Making this line long enough for the example.");
            }
        }

        End();
    }

    private static void ApplySizeConstraintOption(SizeConstraintOption option)
    {
        if (option.CustomCallback != null)
        {
            SetNextWindowSizeConstraints(option.SizeMin, option.SizeMax, option.CustomCallback);
        }
        else
        {
            SetNextWindowSizeConstraints(option.SizeMin, option.SizeMax);
        }
    }

    private static unsafe void AspectRatioSizeCallback(float aspectRatio, ImGuiSizeCallbackData* data)
    {
        data->DesiredSize.Y = (int)(data->DesiredSize.X / aspectRatio);
    }

    private static unsafe void SquareSizeCallback(ImGuiSizeCallbackData* data)
    {
        data->DesiredSize.X = data->DesiredSize.Y = Math.Max(data->DesiredSize.X, data->DesiredSize.Y);
    }

    private static unsafe void StepSizeCallback(float step, ImGuiSizeCallbackData* data)
    {
        data->DesiredSize = new((int)(data->DesiredSize.X / step + 0.5f) * step, (int)(data->DesiredSize.Y / step + 0.5f) * step);
    }

    private record SizeConstraintOption(string Description, Vector2 SizeMin, Vector2 SizeMax, ImGuiSizeCallback CustomCallback);
}
