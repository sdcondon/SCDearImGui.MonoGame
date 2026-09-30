using ImGuiNET;
using static ImGuiNET.ImGui;
using static SCDearImGui.MonoGame.Demos.GuiElements.GuiElementHelpers;

namespace SCDearImGui.MonoGame.Demos.GuiElements.DemoWindow;

class DemoWindowInputsNavAndFocusSection
{
    private string dummyTextBoxContent = "hello";

    public void Update()
    {
        if (!CollapsingHeader("Inputs, Navigation & Focus")) return;

        ImGuiIOPtr io = GetIO();

        Text($"WantCaptureMouse: {io.WantCaptureMouse}");
        Text($"WantCaptureKeyboard: {io.WantCaptureKeyboard}");
        Text($"WantTextInput: {io.WantTextInput}");
        Text($"WantSetMousePos: {io.WantSetMousePos}");
        Text($"NavActive: {io.NavActive}, NavVisible: {io.NavVisible}");

        UpdateKeyboardMouseAndNavStateSubsection(io);
        UpdateTabbingSubsection();
    }

    private static void UpdateKeyboardMouseAndNavStateSubsection(ImGuiIOPtr io)
    {
        if (!TreeNode("Keyboard, Mouse & Navigation State")) return;

        if (IsMousePosValid())
        {
            Text(string.Format("Mouse pos: ({0}, {1})", io.MousePos.X, io.MousePos.Y));
        }
        else
        {
            Text("Mouse pos: <INVALID>");
        }

        Text(string.Format("Mouse delta: ({0}, {1})", io.MouseDelta.X, io.MouseDelta.Y));
        Text("Mouse down:"); for (int i = 0; i < io.MouseDown.Count; i++) if (io.MouseDownDuration[i] >= 0.0f) { SameLine(); Text(string.Format("{0} ({1} secs)", i, io.MouseDownDuration[i])); }
        Text("Mouse clicked:"); for (int i = 0; i < io.MouseDown.Count; i++) if (IsMouseClicked((ImGuiMouseButton)i)) { SameLine(); Text(i.ToString()); }
        Text("Mouse dblclick:"); for (int i = 0; i < io.MouseDown.Count; i++) if (IsMouseDoubleClicked((ImGuiMouseButton)i)) { SameLine(); Text(i.ToString()); }
        Text("Mouse released:"); for (int i = 0; i < io.MouseDown.Count; i++) if (IsMouseReleased((ImGuiMouseButton)i)) { SameLine(); Text(i.ToString()); }
        Text(string.Format("Mouse wheel: {0}", io.MouseWheel));

        // TODO: Not Supported
        /*
        Text("Keys down:"); for (int i = 0; i < io.KeysDown.Count; i++) if (io.KeysDownDuration[i] >= 0.0f) { SameLine(); Text(string.Format("{0} ({1}) ({2} secs)", i, i, io.KeysDownDuration[i])); }
        Text("Keys pressed:"); for (int i = 0; i < io.KeysDown.Count; i++) if (IsKeyPressed((ImGuiKey)i)) { SameLine(); Text(string.Format("{0} ({1})", i, i)); }
        Text("Keys release:"); for (int i = 0; i < io.KeysDown.Count; i++) if (IsKeyReleased((ImGuiKey)i)) { SameLine(); Text(string.Format("{0} ({1})", i, i)); }
        Text(string.Format("Keys mods: {0}{1}{2}{3}", io.KeyCtrl ? "CTRL " : "", io.KeyShift ? "SHIFT " : "", io.KeyAlt ? "ALT " : "", io.KeySuper ? "SUPER " : ""));
        Text("Chars queue:"); for (int i = 0; i < io.InputQueueCharacters.Size; i++) { ushort c = io.InputQueueCharacters[i]; SameLine(); Text(string.Format("{0} {1}", (c > ' ' && c <= 255) ? (char)c : '?', c)); } // FIXME: Does not show chars as in example

        Text("NavInputs down:"); for (int i = 0; i < io.NavInputs.Count; i++) if (io.NavInputs[i] > 0.0f) { SameLine(); Text(string.Format("[{0}] {1}", i, io.NavInputs[i])); }
        Text("NavInputs pressed:"); for (int i = 0; i < io.NavInputs.Count; i++) if (io.NavInputsDownDuration[i] == 0.0f) { SameLine(); Text(string.Format("[{0}]", i)); }
        Text("NavInputs duration:"); for (int i = 0; i < io.NavInputs.Count; i++) if (io.NavInputsDownDuration[i] >= 0.0f) { SameLine(); Text(string.Format("[{0}] {1}", i, io.NavInputsDownDuration[i])); }

        Button("Hovering me sets the\nkeyboard capture flag");
        if (IsItemHovered())
        {
            // ..
        }
        SameLine();
        Button("Holding me clears the\nthe keyboard capture flag");
        if (IsItemActive())
        {
            // ImGui.CaptureKeyboardFromApp(true);
        }
        */

        TreePop();
    }

    private void UpdateTabbingSubsection()
    {
        if (!TreeNode("Tabbing")) return;

        Text("Use TAB/SHIFT+TAB to cycle through keyboard editable fields.");
        InputText("1", ref dummyTextBoxContent, 100);
        InputText("2", ref dummyTextBoxContent, 100);
        InputText("3", ref dummyTextBoxContent, 100);
        // TODO: Not Supported
        // PushAllowKeyboardFocus(false);
        InputText("4 (tab skip)", ref dummyTextBoxContent, 100);
        // TODO: Not Supported
        // PopAllowKeyboardFocus();
        InputText("5", ref dummyTextBoxContent, 100);

        TreePop();
    }
}
