using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using static ImGuiNET.ImGui;

namespace SCDearImGui.MonoGame.Demos.GuiElements.Concepts;

// Demonstrate/test rendering huge amount of text, and the incidence of clipping.
// Original: https://github.com/ocornut/imgui/blob/master/imgui_demo.cpp#L9517
class LongTextDisplayWindow(bool isOpen = false)
{
    public bool IsOpen = isOpen;

    private readonly List<string> lines = [];
    private int test_type = 0;

    public void Update()
    {
        if (!IsOpen) return;

        SetNextWindowSize(new(520, 600), ImGuiCond.FirstUseEver);
        if (!Begin("Example: Long text display", ref IsOpen))
        {
            End();
            return;
        }

        Text("Printing unusually long amount of text.");

        Combo("Test type", ref test_type,
            "Single call to TextUnformatted()\0"
            + "Multiple calls to Text(), clipped\0"
            + "Multiple calls to Text(), not clipped (slow)\0");

        Text($"Buffer contents: {lines.Count} lines");

        if (Button("Clear"))
        {
            lines.Clear();
        }

        SameLine();
        if (Button("Add 1000 lines"))
        {
            for (int i = 0; i < 1000; i++)
                lines.Add($"{lines.Count} The quick brown fox jumps over the lazy dog");
        }

        BeginChild("Log");
        switch (test_type)
        {
            case 0:
                // Single call to TextUnformatted() with a big buffer
                TextUnformatted(string.Join("\n", lines));
                break;
            case 1:
                // Multiple calls to Text(), manually coarsely clipped - demonstrate how to use the ImGuiListClipper helper.
                PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
                unsafe
                {
                    ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
                    try
                    {
                        clipper.Begin(lines.Count);
                        while (clipper.Step())
                        {
                            for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                            {
                                Text(lines[i]);
                            }
                        }
                    }
                    finally
                    {
                        clipper.Destroy();
                    }
                }
                PopStyleVar();
                break;
            case 2:
                // Multiple calls to Text(), not clipped (slow)
                PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
                for (int i = 0; i < lines.Count; i++)
                    Text(lines[i]);
                PopStyleVar();
                break;
        }
        EndChild();

        End();
    }
}
