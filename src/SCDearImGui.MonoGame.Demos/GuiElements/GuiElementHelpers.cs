using ImGuiNET;
using System;
using System.Runtime.CompilerServices;

namespace SCDearImGui.MonoGame.Demos.GuiElements;

/// <summary>
/// Assorted common helper methods used by our GUI elements.
/// </summary>
static class GuiElementHelpers
{
    public static void HelpMarker(string text)
    {
        var buttonColor = ImGui.GetStyle().Colors[(int)ImGuiCol.ButtonHovered];
        ImGui.TextColored(buttonColor, "(?)");
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
            ImGui.TextUnformatted(text);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }

    public static bool CheckboxFlags<T>(string label, ref T flags, T value)
        where T : struct, Enum
    {
        // Yeah, there is some boxing and switching on underlying types in here, and performance is
        // a valid concern given how often this will be called.
        // This is just a price we pay for genericness. It is of course trivial to create a
        // non-generic version of this helper specific to a particular enum type if this is a problem.
        // Or of course some kind of instantiable CheckboxFlagsProvider<TEnum> class that does some
        // work once upon instantiation to create some private delegates that are more efficient,
        // and exposes a bool CheckboxFlags(string, ref TEnum, TEnum) instance method.
        var underlyingType = Enum.GetUnderlyingType(typeof(T));
        var flagsAsUnderlyingType = Convert.ChangeType(flags, underlyingType);
        var valueAsUnderlyingType = Convert.ChangeType(value, underlyingType);

        bool returnValue;
        switch (flagsAsUnderlyingType, valueAsUnderlyingType)
        {
            case (int flagsAsInt, int valueAsInt):
                returnValue = ImGui.CheckboxFlags(label, ref flagsAsInt, valueAsInt);
                flags = (T)Enum.ToObject(typeof(T), flagsAsInt);
                break;

            case (uint flagsAsUInt, uint valueAsUInt):
                returnValue = ImGui.CheckboxFlags(label, ref flagsAsUInt, valueAsUInt);
                flags = (T)Enum.ToObject(typeof(T), flagsAsUInt);
                break;

            default:
                throw new NotSupportedException($"Enums with underlying type {flagsAsUnderlyingType.GetType().Name} are not supported");
        }

        return returnValue;
    }

    public static bool CheckboxFlags<T>(ref T flags, T value)
        where T : struct, Enum
    {
        return CheckboxFlags(value.ToString(), ref flags, value);
    }

    /// <summary>
    /// <para>
    /// Submits a text element to ImGui, pulling content from an interpolated string without 
    /// necessarily making a heap allocation - via use of the <see cref="DefaultInterpolatedStringHandler"/>.
    /// </para>
    /// <para>
    /// NB: we use the handler used by the compiler to resolve interpolated strings rather than
    /// creating our own. This keeps the work to a minimum for me (this is only sa demo proj, after
    /// all), but does mean that in error scenarios the exceptions thrown might not be as graceful
    /// as if we'd written our own handler.
    /// </para>
    /// </summary>
    public static void Text(DefaultInterpolatedStringHandler interpolatedStringHandler)
    {
        ImGui.Text(interpolatedStringHandler.Text);
        interpolatedStringHandler.Clear();
    }

    public static void ExampleFileMenu()
    {
        ImGui.MenuItem("(demo menu)", null, false, false);
        if (ImGui.MenuItem("New")) { }
        if (ImGui.MenuItem("Open", "Ctrl+O")) { }
        if (ImGui.MenuItem("Open Recent"))
        {
            ImGui.MenuItem("fish_hat.c");
            ImGui.MenuItem("fish_hat.inl");
            ImGui.MenuItem("fish_hat.h");
            if (ImGui.MenuItem("More.."))
            {
                ImGui.MenuItem("Hello");
                ImGui.MenuItem("Sailor");
                if (ImGui.BeginMenu("Recurse.."))
                {
                    ExampleFileMenu();
                    ImGui.EndMenu();
                }
                ImGui.EndMenu();
            }
            ImGui.EndMenu();
        }
        if (ImGui.MenuItem("Save", "Ctrl+S")) { }
        if (ImGui.MenuItem("Save As ..")) { }

        ImGui.Separator();
        if (ImGui.BeginMenu("Options"))
        {
            bool enabled = true;
            ImGui.MenuItem("Enabled", "", enabled);
            ImGui.BeginChild("child", new(0, 60), ImGuiChildFlags.Borders);
            for (int i = 0; i < 10; i++)
            {
                ImGui.Text(string.Format("Scrolling Text {0}", i));
            }
            ImGui.EndChild();
            float f = 0.5f;
            int n = 0;
            ImGui.SliderFloat("Value", ref f, 0.0f, 1.0f);
            ImGui.InputFloat("Input", ref f, 0.1f);
            ImGui.Combo("Combo", ref n, "Yes\0No\0Maybe\0\0");
            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Colors"))
        {
            float sz = ImGui.GetTextLineHeight();
            //ImGui.Text(((int)ImGuiCol.COUNT).ToString()); //Test
            for (int i = 0; i < (int)ImGuiCol.COUNT; i++)
            {
                string name = ImGui.GetStyleColorName((ImGuiCol)i);
                var p = ImGui.GetCursorScreenPos();
                ImGui.GetWindowDrawList().AddRectFilled(p, new(p.X + sz, p.Y + sz), ImGui.GetColorU32((ImGuiCol)i));
                ImGui.Dummy(new(sz, sz));
                ImGui.SameLine();
                ImGui.MenuItem(name);
            }
            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Options")) //Append!
        {
            bool b = true;
            ImGui.Checkbox("SomeOption", ref b);
            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Disabled", false)) { } //Disabled
        if (ImGui.MenuItem("Checked", null, true)) { }
        if (ImGui.MenuItem("Quit", "Alt+F4")) { }
    }

    public static void PushStyleCompact()
    {
        // Make the UI compact - useful when we have a lot of fields to display
        ImGuiStylePtr style = ImGui.GetStyle();
        ImGui.PushStyleVarY(ImGuiStyleVar.FramePadding, (float)(int)(style.FramePadding.Y * 0.60f));
        ImGui.PushStyleVarY(ImGuiStyleVar.ItemSpacing, (float)(int)(style.ItemSpacing.Y * 0.60f));
    }

    public static void PopStyleCompact()
    {
        ImGui.PopStyleVar(2);
    }

    ////public static void DrawAvailRect()
    ////{
    ////    var vMin = ImGui.GetCursorPos(); // cursor pos within window
    ////    var vMax = ImGui.GetCursorPos() + ImGui.GetContentRegionAvail();

    ////    vMin += ImGui.GetWindowPos();
    ////    vMax += ImGui.GetWindowPos();

    ////    ImGui.GetForegroundDrawList().AddRect(vMin, vMax, ImGui.ColorConvertFloat4ToU32(new Vec4(255.0f, 255.0f, 0.0f, 100.0f)));
    ////}
}
