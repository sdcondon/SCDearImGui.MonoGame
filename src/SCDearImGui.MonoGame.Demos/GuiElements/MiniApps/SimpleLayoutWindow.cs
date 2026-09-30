using ImGuiNET;
using System.Numerics;
using static ImGuiNET.ImGui;
using static SCDearImGui.MonoGame.Demos.GuiElements.GuiElementHelpers;

namespace SCDearImGui.MonoGame.Demos.GuiElements.MiniApps;

// Example window with multiple child windows.
class SimpleLayoutWindow(bool isOpen = false)
{
    public bool IsOpen = isOpen;

    private int selectedItemIndex = 0;

    public void Update()
    {
        if (!IsOpen) return;

        SetNextWindowSize(new Vector2(500, 440), ImGuiCond.FirstUseEver);

        if (Begin("Example: Simple layout", ref IsOpen, ImGuiWindowFlags.MenuBar))
        {
            // Menu bar
            if (BeginMenuBar())
            {
                if (BeginMenu("File"))
                {
                    if (MenuItem("Close", "Ctrl+W"))
                    {
                        IsOpen = false;
                    }

                    EndMenu();
                }

                EndMenuBar();
            }

            // Left pane - single child window
            BeginChild("left pane", new Vector2(150, 0), ImGuiChildFlags.Borders | ImGuiChildFlags.ResizeX);
            for (int i = 0; i < 100; i++)
            {
                if (Selectable($"MyObject {i}", selectedItemIndex == i))
                {
                    selectedItemIndex = i;
                }
            }
            EndChild();

            // Right pane - group consisting of child window and some buttons below
            SameLine();
            BeginGroup();
            BeginChild("item view", new Vector2(0, -GetFrameHeightWithSpacing())); // Leave room for 1 line below us
            Text($"MyObject: {selectedItemIndex}");
            Separator();
            if (BeginTabBar("##Tabs", ImGuiTabBarFlags.None))
            {
                if (BeginTabItem("Description"))
                {
                    TextWrapped("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. ");
                    EndTabItem();
                }
                if (BeginTabItem("Details"))
                {
                    Text("ID: 0123456789");
                    EndTabItem();
                }
                EndTabBar();
            }
            EndChild();

            if (Button("Revert")) { }
            SameLine();

            if (Button("Save")) { }
            EndGroup();
        }

        End();
    }
}
