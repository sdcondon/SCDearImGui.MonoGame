using ImGuiNET;
using System;
using static ImGuiNET.ImGui;
using static SCDearImGui.MonoGame.Demos.GuiElements.GuiElementHelpers;
using Vec2 = System.Numerics.Vector2;
using Vec3 = System.Numerics.Vector3;
using Vec4 = System.Numerics.Vector4;

namespace SCDearImGui.MonoGame.Demos.GuiElements.DemoWindow;

class DemoWindowWidgetsSection()
{
    private readonly BasicInputSubsection basicInputSubsection = new();
    private readonly TreesSubsection treesSubsection = new();
    private readonly CollapsingHeadersSubsection collapsingHeadersSubsection = new();
    private readonly TextSubsection textSubsection = new();
    private readonly ImagesSubsection imagesSubsection = new();
    private readonly ComboSubsection comboSubsection = new();
    private readonly ListBoxesSubsection listBoxesSubsection = new();
    private readonly SelectablesSubsection selectablesSubsection = new();
    private readonly TextInputSubsection textInputSubsection = new();
    private readonly TabsSubsection tabsSubsection = new();
    private readonly PlotsWidgetsSubsection plotsWidgetsSubsection = new();
    private readonly ColorPickerWidgetsSubsection colorPickerWidgetsSubsection = new();
    private readonly RangeWidgetsSubsection rangeWidgetsSubsection = new();
    private readonly MultiComponentWidgetsSubsection multiComponentWidgetsSubsection = new();
    private readonly VerticalSlidersSubsection verticalSlidersSubsection = new();

    public void Update()
    {
        if (!CollapsingHeader("Widgets")) return;

        basicInputSubsection.Update();
        treesSubsection.Update();
        collapsingHeadersSubsection.Update();
        UpdateBulletsSubsection();
        textSubsection.Update();
        imagesSubsection.Update();
        comboSubsection.Update();
        listBoxesSubsection.Update();
        selectablesSubsection.Update();
        textInputSubsection.Update();
        tabsSubsection.Update();
        plotsWidgetsSubsection.Update();
        colorPickerWidgetsSubsection.Update();
        rangeWidgetsSubsection.Update();
        multiComponentWidgetsSubsection.Update();
        verticalSlidersSubsection.Update();
    }

    private static void UpdateBulletsSubsection()
    {
        if (!TreeNode("Bullets")) return;

        BulletText("Bullet point 1");
        BulletText("Bullet point 2\nOn multiple lines");
        if (TreeNode("Tree node"))
        {
            BulletText("Another bullet point");
            TreePop();
        }
        Bullet();
        Text("Bullet point 3 (two calls)");
        Bullet();
        SmallButton("Button");

        TreePop();
    }

    private class BasicInputSubsection
    {
        int clicked = 0;
        bool check = true;
        int e = 0;
        int counter = 0;
        int item_current = 0;
        string str0 = "Hello, world!";
        string str1 = "";
        int i0 = 123;
        float f0 = 0.001f;
        double d0 = 999999.00000001;
        float f1 = 1.42f;
        Vec3 vec3 = new(0.10f, 0.20f, 0.30f);

        int i1 = 50, i2 = 42;
        float f2 = 1.00f, f3 = 0.0067f;
        int i3 = 0;
        float f4 = 0.123f;
        float angle = 0.0f;
        Vec3 col1 = new(1.0f, 0.0f, 0.2f);
        Vec4 col2 = new(0.4f, 0.7f, 0.0f, 0.5f);
        int current_fruit = 1;

        public void Update()
        {
            if (!TreeNode("Basic")) return;

            if (Button("Button"))
            {
                clicked++;
                if (clicked == 2)
                {
                    clicked = 0;
                }
            }
            if (clicked == 1)
            {
                SameLine();
                Text("Thanks for clicking me!");
            }

            Checkbox("checkbox", ref check);

            RadioButton("radio a", ref e, 0);
            SameLine();
            RadioButton("radio b", ref e, 1);
            SameLine();
            RadioButton("radio c", ref e, 2);

            for (int i = 0; i < 7; i++)
            {
                if (i > 0)
                {
                    SameLine();
                }
                PushID(i);
                //ImColorPtr color = new ImColorPtr();
                //ImGuiNative.ImColor_HSV ?
                PushStyleColor(ImGuiCol.Button, new Vec4(i / 7.0f, 0.6f, 0.6f, 0.6f));
                PushStyleColor(ImGuiCol.ButtonHovered, new Vec4(i / 7.0f, 0.7f, 0.7f, 0.7f));
                PushStyleColor(ImGuiCol.ButtonActive, new Vec4(i / 7.0f, 0.8f, 0.8f, 0.8f));
                Button("Click");
                PopStyleColor(3);
                PopID();
            }

            AlignTextToFramePadding();
            Text("Hold to repeat:");
            SameLine();

            float spacing = GetStyle().ItemInnerSpacing.X;
            PushItemFlag(ImGuiItemFlags.ButtonRepeat, true);
            if (ArrowButton("##left", ImGuiDir.Left)) { counter--; }
            SameLine(0.0f, spacing);
            if (ArrowButton("##right", ImGuiDir.Right)) { counter++; }
            PopItemFlag();
            SameLine();
            Text(counter.ToString());

            Text("Hover over me");
            if (IsItemHovered())
            {
                SetTooltip("I am a tooltip");
            }

            SameLine();
            Text("- or me");
            if (IsItemHovered())
            {
                BeginTooltip();
                Text("I am a fancy tooltip");
                float[] arr = [0.6f, 0.1f, 1.0f, 0.5f, 0.92f, 0.1f, 0.2f];
                PlotLines("Curve", ref arr[0], arr.Length);
                EndTooltip();
            }

            Separator();
            LabelText("label", "Value");

            //combo box
            string items = "AAAA\0BBBB\0CCCC\0DDDD\0EEEE\0FFFF\0GGGG\0HHHH\0IIIIIII\0JJJJ\0KKKKKKK";
            Combo("combo", ref item_current, items, 11);

            //input
            InputText("input text", ref str0, 128);
            SameLine();
            HelpMarker("USER:\n" +
            "Hold SHIFT or use mouse to select text.\n" +
            "CTRL+Left/Right to word jump.\n" +
            "CTRL+A or double-click to select all.\n" +
            "CTRL+X,CTRL+C,CTRL+V clipboard.\n" +
            "CTRL+Z,CTRL+Y undo/redo.\n" +
            "ESCAPE to revert.\n\n" +
            "PROGRAMMER:\n" +
            "You can use the ImGuiInputTextFlags_CallbackResize facility if you need to wire InputText() " +
            "to a dynamic string type. See misc/cpp/imgui_stdlib.h for an example (this is not demonstrated " +
            "in imgui_demo.cpp).");

            InputTextWithHint("input text (w/ hint", "enter text here", ref str1, 10);

            InputInt("input int", ref i0);
            SameLine();
            HelpMarker("You can apply arithmetic operators +,*,/ on numerical values.\n" +
                        "  e.g. [ 100 ], input \'*2\', result becomes [ 200 ]\n" +
                        "Use +- to subtract.");

            InputFloat("input float", ref f0, 0.01f, 1.0f, "%.3f");

            InputDouble("input double", ref d0, 0.01f, 1.0f, "%.8f");

            InputFloat("input scientific", ref f1, 0.0f, 0.0f, "%e");
            SameLine();
            HelpMarker("You can input value using the scientific notation,\n" +
                        "  e.g. \"1e+8\" becomes \"100000000\".");

            InputFloat3("input float 3", ref vec3);

            //drag               
            DragInt("drag int", ref i1, 1);
            SameLine();
            HelpMarker("Click and drag to edit value.\n" +
            "Hold SHIFT/ALT for faster/slower edit.\n" +
            "Double-click or CTRL+click to input value.");

            DragInt("drag int 0..100", ref i2, 1, 0, 100, "%d%%");

            DragFloat("drag float", ref f2, 0.005f);
            DragFloat("drag small float", ref f3, 0.0001f, 0.0f, 0.0f, "%.06f ns");

            SliderInt("slider int", ref i3, -1, 3);
            SameLine();
            HelpMarker("CTRL+click to input value.");

            SliderFloat("slider float", ref f4, 0.0f, 1.0f, "ratio = %.3f");

            SliderAngle("slider angle", ref angle);

            //color
            ColorEdit3("color 1", ref col1);
            SameLine();
            HelpMarker("Click on the color square to open a color picker.\n" +
            "Click and hold to use drag and drop.\n" +
            "Right-click on the color square to show options.\n" +
            "CTRL+click on individual component to input value.\n");

            ColorEdit4("color 2", ref col2);

            string[] fruits = ["Apple", "Banana", "Cherry", "Kiwi", "Mango", "Orange", "Pineapple", "Strawberry", "Watermelon"];
            ListBox("listbox", ref current_fruit, fruits, fruits.Length);
            SameLine();
            HelpMarker("Using the simplified one-liner ListBox API here.\nRefer to the \"List boxes\" section below for an explanation of how to use the more flexible and general BeginListBox/EndListBox API.");

            TreePop();
        }
    }

    private class TreesSubsection
    {
        bool base_flags_first_run = true;
        ImGuiTreeNodeFlags base_flags = 0;
        bool align_label_with_current_x_position = false;
        int index_selected = 0;
        //bool test_drag_and_drop = false;

        public void Update()
        {
            if (!TreeNode("Trees")) return;

            if (TreeNode("Basic trees"))
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        SetNextItemOpen(true, ImGuiCond.Once);
                    }

                    if (TreeNode(i.ToString(), "Child " + i.ToString()))
                    {
                        Text("blah blah");
                        SameLine();
                        if (SmallButton("button")) { }
                        TreePop();
                    }
                }
                TreePop();
            }

            if (TreeNode("Advanced, with Selectable nodes"))
            {
                HelpMarker(
                    "This is a more typical looking tree with selectable nodes.\n" +
                    "Click to select, CTRL+Click to toggle, click on arrows or double-click to open.");

                if (base_flags_first_run)
                {
                    ImGuiTreeNodeFlags _base_flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick;
                    base_flags = _base_flags;
                    base_flags_first_run = false;
                }

                CheckboxFlags(ref base_flags, ImGuiTreeNodeFlags.OpenOnArrow);
                CheckboxFlags(ref base_flags, ImGuiTreeNodeFlags.OpenOnDoubleClick);
                Checkbox("Align label with current X position", ref align_label_with_current_x_position);
                //Checkbox("Test tree node as drag source", ref test_drag_and_drop);

                Text("Hello!");
                if (align_label_with_current_x_position)
                {
                    Unindent(GetTreeNodeToLabelSpacing());
                }

                for (int i = 0; i < 6; i++)
                {
                    ImGuiTreeNodeFlags node_flags = base_flags;
                    if (i == index_selected)
                    {
                        node_flags |= ImGuiTreeNodeFlags.Selected;
                    }

                    if (i < 3)
                    {
                        if (TreeNodeEx(i.ToString(), node_flags, "Selectable Node " + i.ToString()))
                        {
                            BulletText("Blah blah\nBlah Blah");
                            TreePop();
                        }
                    }
                    else
                    {
                        node_flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
                        TreeNodeEx(i.ToString(), node_flags, "Selectable Leaf " + i.ToString());
                    }

                    if (IsItemClicked())
                    {
                        index_selected = i;
                    }
                }


                TreePop();

            }

            TreePop();
        }
    }

    private class CollapsingHeadersSubsection
    {
        bool closable_group = true;

        public void Update()
        {
            if (!TreeNode("Collapsing Headers")) return;

            Checkbox("Show 2nd header", ref closable_group);

            if (CollapsingHeader("Header", ImGuiTreeNodeFlags.None))
            {
                Text("IsItemHovered: " + IsItemHovered());
                for (int i = 0; i < 5; i++)
                {
                    Text("Some content " + i);
                }
            }

            if (CollapsingHeader("Header with a close button", ref closable_group))
            {
                Text("IsItemHovered: " + IsItemHovered());
                for (int i = 0; i < 5; i++)
                {
                    Text("More content " + i);
                }
            }

            TreePop();
        }
    }

    private class TextSubsection
    {
        float wrap_width = 200.0f;

        public void Update()
        {
            if (!TreeNode("Text")) return;

            if (TreeNode("Colorful Text"))
            {
                TextColored(new Vec4(1.0f, 0.0f, 1.0f, 1.0f), "Pink");
                TextColored(new Vec4(1.0f, 1.0f, 0.0f, 1.0f), "Yellow");
                TextDisabled("Disabled");
                SameLine();
                HelpMarker("The TextDisabled color is stored in ImGuiStyle.");
                TreePop();
            }

            if (TreeNode("Word Wrapping"))
            {
                TextWrapped("This text should automatically wrap on the edge of the window. The current implementation " +
                "for text wrapping follows simple rules suitable for English and possibly other languages.");
                Spacing();

                SliderFloat("Wrap width", ref wrap_width, -20, 600, "%.0f");

                ImDrawListPtr draw_list = GetWindowDrawList();
                for (int n = 0; n < 2; n++)
                {
                    Text("Test paragraph " + n);
                    Vec2 pos = GetCursorPos();
                    Vec2 marker_min = new(pos.X + wrap_width, pos.Y);
                    Vec2 marker_max = new(pos.X + wrap_width + 10, pos.Y + GetTextLineHeight());
                    PushTextWrapPos(GetCursorPos().X + wrap_width);
                    if (n == 0)
                    {
                        Text(string.Format("The lazy dog is a good dog. This paragraph should fit within {0} pixels. Testing a 1 character word. The quick brown fox jumps over the lazy dog.", wrap_width));
                    }
                    else
                    {
                        Text("aaaaaaaa bbbbbbbb, c cccccccc,dddddddd. d eeeeeeee   ffffffff. gggggggg!hhhhhhhh");
                    }

                    Vec4 colf = new(255.0f, 255.0f, 0.0f, 255.0f);
                    uint col = ColorConvertFloat4ToU32(colf); //ImGuiNative.igColorConvertFloat4ToU32(colf);

                    draw_list.AddRect(GetItemRectMin(), GetItemRectMax(), col);

                    Vec4 colf2 = new(255.0f, 0.0f, 255.0f, 255.0f);
                    uint col2 = ColorConvertFloat4ToU32(colf2); //ImGuiNative.igColorConvertFloat4ToU32(colf2);

                    draw_list.AddRectFilled(marker_min, marker_max, col2);

                    PopTextWrapPos();

                }
                TreePop();
            }

            TreePop();
        }
    }

    private class ImagesSubsection
    {
        int pressed_count = 0;

        public void Update()
        {
            if (!TreeNode("Images")) return;

            ImGuiIOPtr io = GetIO();
            TextWrapped("Below we are displaying the font texture (which is the only texture we have access to in this demo). " +
                "Use the 'ImTextureID' type as storage to pass pointers or identifier to your own texture data. " +
                "Hover the texture for a zoomed view!");

            System.IntPtr my_tex_id = io.Fonts.TexID;
            float my_tex_w = (float)io.Fonts.TexWidth;
            float my_tex_h = (float)io.Fonts.TexHeight;

            Text(my_tex_w + "x" + my_tex_h);
            Vec2 pos = GetCursorScreenPos();
            Vec2 uv_min = new(0.0f, 0.0f); // top left
            Vec2 uv_max = new(1.0f, 1.0f); // lower right
            Vec4 tint_col = new(1.0f, 1.0f, 1.0f, 1.0f); // no tint
            Vec4 border_col = new(1.0f, 1.0f, 1.0f, 0.5f); // 50% opaque white

            Image(my_tex_id, new Vec2(my_tex_w, my_tex_h), uv_min, uv_max, tint_col, border_col);

            if (IsItemHovered())
            {
                BeginTooltip();
                float region_sz = 32.0f;
                float region_x = io.MousePos.X - pos.X - region_sz * 0.5f;
                float region_y = io.MousePos.Y - pos.Y - region_sz * 0.5f;
                float zoom = 4.0f;
                if (region_x < 0.0f) { region_x = 0.0f; }
                else if (region_x > my_tex_w - region_sz) { region_x = my_tex_w - region_sz; }
                if (region_y < 0.0f) { region_y = 0.0f; }
                else if (region_y > my_tex_h - region_sz) { region_y = my_tex_h - region_sz; }
                Text(string.Format("Min: ({0},{1})", region_x, region_y));
                Text(string.Format("Max: ({0},{1})", region_x + region_sz, region_y + region_sz));
                Vec2 uv0 = new((region_x) / my_tex_w, (region_y) / my_tex_h);
                Vec2 uv1 = new((region_x + region_sz) / my_tex_w, (region_y + region_sz) / my_tex_h);
                Image(my_tex_id, new Vec2(region_sz * zoom, region_sz * zoom), uv0, uv1, tint_col, border_col);
                EndTooltip();
            }

            TextWrapped("And now some textured buttons..");
            for (int i = 0; i < 8; i++)
            {
                PushID(i);
                //int frame_padding = -1 + i;
                Vec2 size = new(32.0f, 32.0f);
                Vec2 uv0 = new(0.0f, 0.0f);
                Vec2 uv1 = new(32.0f / my_tex_w, 32.0f / my_tex_h);
                Vec4 bg_col = new(0.0f, 0.0f, 0.0f, 1.0f); // black background
                Vec4 tint_col2 = new(1.0f, 1.0f, 1.0f, 1.0f); // no tint
                if (ImageButton($"textured_button_id_{i}", my_tex_id, size, uv0, uv1, bg_col, tint_col2))
                {
                    pressed_count += 1;
                }
                PopID();
                SameLine();
            }
            NewLine();
            Text(string.Format("Pressed {0} times.", pressed_count));
            TreePop();
        }
    }

    private class ComboSubsection
    {
        ImGuiComboFlags flags = 0;
        int item_current_idx = 0;
        int item_current_2 = 0;
        int item_current_3 = -1;

        public void Update()
        {
            if (!TreeNode("Combo")) return;

            CheckboxFlags(ref flags, ImGuiComboFlags.PopupAlignLeft);
            SameLine();
            HelpMarker("Only makes a difference if the popup is larger than the combo");

            if (CheckboxFlags(ref flags, ImGuiComboFlags.NoArrowButton))
            {
                flags &= ~ImGuiComboFlags.NoPreview; // clear the other flag
            }

            if (CheckboxFlags(ref flags, ImGuiComboFlags.NoPreview))
            {
                flags &= ~ImGuiComboFlags.NoArrowButton; // clear the other flag
            }

            string[] items = ["AAAA", "BBBB", "CCCC", "DDDD", "EEEE", "FFFF", "GGGG", "HHHH", "IIII", "JJJJ", "KKKK", "LLLLLLL", "MMMM", "OOOOOOO"];
            string combo_label = items[item_current_idx];
            ImGuiComboFlags flags_cf = flags;
            if (BeginCombo("combo 1", combo_label, flags_cf))
            {
                for (int n = 0; n < items.Length; n++)
                {
                    bool is_selected = (item_current_idx == n);
                    if (Selectable(items[n], is_selected))
                    {
                        item_current_idx = n;
                    }

                    if (is_selected)
                    {
                        SetItemDefaultFocus();
                    }
                }
                EndCombo();
            }

            Combo("combo 2 (one-liner)", ref item_current_2, "aaaa\0bbbb\0cccc\0dddd\0eeee\0\0");

            Combo("combo 3 (array)", ref item_current_3, items, items.Length);

            TreePop();
        }

    }

    private class ListBoxesSubsection
    {
        int item_current_idx_lb = 0;

        public void Update()
        {
            if (!TreeNode("List boxes")) return;

            string[] items = ["AAAA", "BBBB", "CCCC", "DDDD", "EEEE", "FFFF", "GGGG", "HHHH", "IIII", "JJJJ", "KKKK", "LLLLLLL", "MMMM", "OOOOOOO"];
            if (ListBox("listbox 1", ref item_current_idx_lb, items, items.Length)) { }

            TreePop();
        }
    }

    private class SelectablesSubsection
    {
        readonly bool[] selection = [false, true, false, false, false];
        int selected = -1;
        readonly bool[] selection_ms = [false, false, false, false, false];
        readonly bool[] selected_rend = [false, false, false];
        readonly bool[] selected_align = [true, false, true, false, true, false, true, false, true];

        public void Update()
        {
            if (!TreeNode("Selectables")) return;

            if (TreeNode("Basic"))
            {
                Selectable("1. I am selectable", ref selection[0]);
                Selectable("2. I am selectable", ref selection[1]);
                Text("3. I am not selectable");
                Selectable("4. I am selectable", ref selection[3]);
                if (Selectable("5. I am double clickable", ref selection[4], ImGuiSelectableFlags.AllowDoubleClick))
                {
                    if (IsMouseDoubleClicked(0))
                    {
                        selection[4] = !selection[4];
                    }
                }
                TreePop();
            }

            if (TreeNode("Selection State: Single Selection"))
            {
                for (int n = 0; n < 5; n++)
                {
                    string buf = string.Format("Object {0}", n);
                    if (Selectable(buf, selected == n))
                    {
                        selected = n;
                    }
                }
                TreePop();
            }

            if (TreeNode("Selection State: Multiple Selection"))
            {
                HelpMarker("Hold CTRL and click to select multiple items.");

                for (int n = 0; n < 5; n++)
                {
                    string buf = string.Format("Object {0}", n);
                    if (Selectable(buf, selection_ms[n]))
                    {
                        if (!GetIO().KeyCtrl)
                        {
                            for (int r = 0; r < selection_ms.Length; r++)
                            {
                                selection_ms[r] = false;
                            }
                        }
                        selection_ms[n] ^= true;
                    }
                }
                TreePop();
            }

            if (TreeNode("Rendering more text into the same line"))
            {
                Selectable("main.c", ref selected_rend[0]); SameLine(300); Text(" 2,345 bytes");
                Selectable("Hello.cpp", ref selected_rend[1]); SameLine(300); Text("12,345 bytes");
                Selectable("Hello.h", ref selected_rend[2]); SameLine(300); Text(" 2,345 bytes");
                TreePop();
            }

            if (TreeNode("Alignment"))
            {
                HelpMarker("By default, Selectables uses style.SelectableTextAlign but it can be overridden on a per-item " +
                    "basis using PushStyleVar(). You'll probably want to always keep your default situation to " +
                    "left-align otherwise it becomes difficult to layout multiple items on a same line");

                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        Vec2 alignment = new((float)x / 2.0f, (float)y / 2.0f);
                        string name = string.Format("({0},{1})", alignment.X, alignment.Y);
                        if (x > 0) { SameLine(); }
                        PushStyleVar(ImGuiStyleVar.SelectableTextAlign, alignment);
                        Selectable(name, ref selected_align[3 * y + x], ImGuiSelectableFlags.None, new Vec2(80, 80));
                        PopStyleVar();
                    }
                }
                TreePop();
            }

            TreePop();
        }
    }

    private class TextInputSubsection
    {
        bool flags_ti_first_run = true;
        ImGuiInputTextFlags flags_ti = 0;
        string buf1 = "";
        string buf2 = "";
        string buf3 = "";
        string buf4 = "";
        string buf5 = "";
        string password = "password123";

        public void Update()
        {
            if (!TreeNode("Text Input")) return;

            if (TreeNode("Multi-line Text Input"))
            {
                string text = "/*\n" +
                " The Pentium F00F bug, shorthand for F0 0F C7 C8,\n" +
                " the hexadecimal encoding of one offending instruction,\n" +
                " more formally, the invalid operand with locked CMPXCHG8B\n" +
                " instruction bug, is a design flaw in the majority of\n" +
                " Intel Pentium, Pentium MMX, and Pentium OverDrive\n" +
                " processors (all in the P5 microarchitecture).\n" +
                "*/\n\n" +
                "label:\n" +
                "\tlock cmpxchg8b eax\n";

                if (flags_ti_first_run)
                {
                    ImGuiInputTextFlags _flags = ImGuiInputTextFlags.AllowTabInput;
                    flags_ti = _flags;
                    flags_ti_first_run = false;
                }

                HelpMarker("You can use the ImGuiInputTextFlags_CallbackResize facility if you need to wire InputTextMultiline() to a dynamic string type. See misc/cpp/imgui_stdlib.h for an example. (This is not demonstrated in imgui_demo.cpp because we don't want to include <string> in here)");
                CheckboxFlags(ref flags_ti, ImGuiInputTextFlags.ReadOnly);
                CheckboxFlags(ref flags_ti, ImGuiInputTextFlags.AllowTabInput);
                CheckboxFlags(ref flags_ti, ImGuiInputTextFlags.CtrlEnterForNewLine);

                ImGuiInputTextFlags flags_ml = flags_ti;
                InputTextMultiline("##source", ref text, 2048, new(0, GetTextLineHeight() * 16), flags_ml);
                TreePop();
            }

            if (TreeNode("Filtered Text Input"))
            {
                InputText("default", ref buf1, (uint)64);
                InputText("decimal", ref buf2, (uint)64, ImGuiInputTextFlags.CharsDecimal);
                InputText("hexadecimal", ref buf3, (uint)64, ImGuiInputTextFlags.CharsHexadecimal);
                InputText("uppercase", ref buf4, (uint)64, ImGuiInputTextFlags.CharsUppercase);
                InputText("no blank", ref buf5, (uint)64, ImGuiInputTextFlags.CharsNoBlank);
                TreePop();
            }

            if (TreeNode("Password Input"))
            {
                InputText("password", ref password, (uint)64, ImGuiInputTextFlags.Password);
                SameLine();
                HelpMarker("Display all characters as '*'.\nDisable clipboard cut and copy.\nDisable logging.\n");
                InputTextWithHint("password (w/ hint)", "<password>", ref password, (uint)64, ImGuiInputTextFlags.Password);
                InputText("password (clear)", ref password, (uint)64);
                TreePop();
            }

            TreePop();
        }
    }

    private class TabsSubsection
    {
        bool flags_tabs_first_run = true;
        ImGuiTabBarFlags tab_bar_flags = 0;
        readonly bool[] opened = [true, true, true, true];

        public void Update()
        {
            if (!TreeNode("Tabs")) return;

            if (TreeNode("Basic"))
            {
                ImGuiTabBarFlags tab_bar_flags = ImGuiTabBarFlags.None;
                if (BeginTabBar("MyTabBar", tab_bar_flags))
                {
                    if (BeginTabItem("Avocado"))
                    {
                        Text("This is the Avocado tab!\nblah blah blah blah blah");
                        EndTabItem();
                    }
                    if (BeginTabItem("Broccoli"))
                    {
                        Text("This is the Broccoli tab!\nblah blah blah blah blah");
                        EndTabItem();
                    }
                    if (BeginTabItem("Cucumber"))
                    {
                        Text("This is the Cucumber tab!\nblah blah blah blah blah");
                        EndTabItem();
                    }
                    EndTabBar();
                }
                Separator();
                TreePop();
            }

            if (TreeNode("Advanced & Close Button"))
            {
                if (flags_tabs_first_run)
                {
                    ImGuiTabBarFlags _tab_bar_flags = ImGuiTabBarFlags.Reorderable;
                    tab_bar_flags = _tab_bar_flags;
                    flags_tabs_first_run = false;
                }

                CheckboxFlags(ref tab_bar_flags, ImGuiTabBarFlags.Reorderable);
                CheckboxFlags(ref tab_bar_flags, ImGuiTabBarFlags.AutoSelectNewTabs);
                CheckboxFlags(ref tab_bar_flags, ImGuiTabBarFlags.TabListPopupButton);
                CheckboxFlags(ref tab_bar_flags, ImGuiTabBarFlags.NoCloseWithMiddleMouseButton);
                if ((tab_bar_flags & ImGuiTabBarFlags.FittingPolicyMask) == 0)
                {
                    tab_bar_flags |= ImGuiTabBarFlags.FittingPolicyDefault;
                }
                if (CheckboxFlags(ref tab_bar_flags, ImGuiTabBarFlags.FittingPolicyResizeDown))
                {
                    tab_bar_flags &= ~(ImGuiTabBarFlags.FittingPolicyMask ^ ImGuiTabBarFlags.FittingPolicyResizeDown);
                }
                if (CheckboxFlags(ref tab_bar_flags, ImGuiTabBarFlags.FittingPolicyScroll))
                {
                    tab_bar_flags &= ~(ImGuiTabBarFlags.FittingPolicyMask ^ ImGuiTabBarFlags.FittingPolicyScroll);
                }

                string[] names = ["Artichoke", "Beetroot", "Celery", "Daikon"];
                for (int n = 0; n < opened.Length; n++)
                {
                    if (n > 0) { SameLine(); }
                    Checkbox(names[n], ref opened[n]);
                }

                ImGuiTabBarFlags tab_bar_flags_tb = tab_bar_flags;
                if (BeginTabBar("MyTabBar", tab_bar_flags_tb))
                {
                    for (int n = 0; n < opened.Length; n++)
                    {
                        if (opened[n] && BeginTabItem(names[n], ref opened[n], ImGuiTabItemFlags.None))
                        {
                            Text(string.Format("This is the {0} tab!", names[n]));
                            if (n == 1 || n == 3)
                            {
                                Text("I am an odd tab.");
                            }
                            EndTabItem();
                        }
                    }
                    EndTabBar();
                }
                Separator();
                TreePop();
            }

            TreePop();
        }
    }

    private class PlotsWidgetsSubsection
    {
        bool animate = true;
        readonly float[] values = new float[90];
        int values_offset = 0;
        double refresh_time = 0.0;
        float phase = 0.0f;
        float progress = 0.0f, progress_dir = 1.0f;

        public void Update()
        {
            if (!TreeNode("Plots Widgets")) return;

            Checkbox("Animate", ref animate);

            float[] arr = [0.6f, 0.1f, 1.0f, 0.5f, 0.92f, 0.1f, 0.2f];
            PlotLines("Frame Times", ref arr[0], arr.Length);

            if (!animate || refresh_time == 0.0)
            {
                refresh_time = GetTime();
            }
            while (refresh_time < GetTime())
            {
                values[values_offset] = (float)Math.Cos(phase);
                values_offset = (values_offset + 1) % values.Length;
                phase += 0.10f * values_offset;
                refresh_time += 1.0f / 60.0f;
            }

            float average = 0.0f;
            for (int n = 0; n < values.Length; n++)
            {
                average += values[n];
            }
            average /= (float)values.Length;
            string overlay = string.Format("avg {0}", average);
            PlotLines("Lines", ref values[0], values.Length, values_offset, overlay, -1.0f, 1.0f, new Vec2(0, 80.0f));
            PlotHistogram("Histogram", ref arr[0], arr.Length, 0, null, 0.0f, 1.0f, new Vec2(0, 80.0f));
            Separator();

            if (animate)
            {
                progress += progress_dir * 0.4f * GetIO().DeltaTime;
                if (progress >= +1.1f) { progress = +1.1f; progress_dir *= -1.0f; }
                if (progress <= -0.1f) { progress = -0.1f; progress_dir *= -1.0f; }
            }

            ProgressBar(progress, new Vec2(0.0f, 0.0f));
            SameLine(0.0f, GetStyle().ItemInnerSpacing.X);
            Text("Progress Bar");

            float progress_saturated = progress; //IM_CLAMP(progress, 0.0f, 1.0f) ?
            string buf = string.Format("{0}/{1}", (int)(progress_saturated * 1753), 1753);
            ProgressBar(progress, new Vec2(0.0f, 0.0f), buf);

            TreePop();
        }
    }

    private class ColorPickerWidgetsSubsection
    {
        //Color Widgets
        Vec3 color_vec3 = new(114.0f / 255.0f, 144.0f / 255.0f, 154 / 255.0f);
        Vec4 color_vec4 = new(114.0f / 255.0f, 144.0f / 255.0f, 154 / 255.0f, 200.0f / 255.0f);
        bool alpha_preview = true;
        bool alpha_half_preview = false;
        bool drag_and_drop = true;
        bool options_menu = true;
        bool hdr = false;
        bool alpha = true;
        bool alpha_bar = true;
        bool side_preview = true;
        bool ref_color = false;
        Vec4 ref_color_v = new(1.0f, 0.0f, 1.0f, 0.5f);
        int display_mode = 0;
        int picker_mode = 0;
        Vec4 color_hsv = new(0.23f, 1.0f, 1.0f, 1.0f);

        public void Update()
        {
            if (!TreeNode("Color/Picker Widgets")) return;

            Checkbox("With Alpha Preview", ref alpha_preview);
            Checkbox("With Half Alpha Preview", ref alpha_half_preview);
            Checkbox("With Drag and Drop", ref drag_and_drop);
            Checkbox("With Options Menu", ref options_menu); SameLine(); HelpMarker("Right-click on the individual color widget to show options.");
            Checkbox("With HDR", ref hdr); SameLine(); HelpMarker("Currently all this does is to lift the 0..1 limits on dragging widgets.");
            ImGuiColorEditFlags misc_flags = (hdr ? ImGuiColorEditFlags.HDR : 0) | (drag_and_drop ? 0 : ImGuiColorEditFlags.NoDragDrop) | (alpha_half_preview ? ImGuiColorEditFlags.AlphaPreviewHalf : (alpha_preview ? ImGuiColorEditFlags.AlphaPreview : 0)) | (options_menu ? 0 : ImGuiColorEditFlags.NoOptions);

            Text("Color widget:");
            SameLine(); HelpMarker(
                "Click on the color square to open a color picker.\n" +
                "CTRL+click on individual component to input value.\n");
            ColorEdit3("MyColor##1", ref color_vec3, misc_flags);

            Text("Color widget HSV with Alpha:");
            ColorEdit4("MyColor##2", ref color_vec4, ImGuiColorEditFlags.DisplayHSV | misc_flags);

            Text("Color widget with Float Display:");
            ColorEdit4("MyColor##2f", ref color_vec4, ImGuiColorEditFlags.Float | misc_flags);

            Text("Color button with Picker:");
            SameLine();
            HelpMarker("With the ImGuiColorEditFlags_NoInputs flag you can hide all the slider/text inputs.\n" +
                "With the ImGuiColorEditFlags_NoLabel flag you can pass a non-empty label which will only " +
                "be used for the tooltip and picker popup.");
            ColorEdit4("MyColor##3", ref color_vec4, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel | misc_flags);

            Text("Color button only:");
            ColorButton("MyColor##3c", color_vec4, misc_flags, new Vec2(80, 80));

            Text("Color picker:");

            Checkbox("With Alpha", ref alpha);
            Checkbox("With Alpha Bar", ref alpha_bar);
            Checkbox("With Side Preview", ref side_preview);
            if (side_preview)
            {
                SameLine();
                Checkbox("With Ref Color", ref ref_color);
                if (ref_color)
                {
                    SameLine();
                    ColorEdit4("##RefColor", ref ref_color_v, ImGuiColorEditFlags.NoInputs | misc_flags);
                }
            }
            Combo("Display Mode", ref display_mode, "Auto/Current\0None\0RGB Only\0HSV Only\0Hex Only\0");
            SameLine();
            HelpMarker(
                "ColorEdit defaults to displaying RGB inputs if you don't specify a display mode, " +
                "but the user can change it with a right-click.\n\nColorPicker defaults to displaying RGB+HSV+Hex " +
                "if you don't specify a display mode.\n\nYou can change the defaults using SetColorEditOptions().");
            Combo("Picker Mode", ref picker_mode, "Auto/Current\0Hue bar + SV rect\0Hue wheel + SV triangle\0");
            SameLine(); HelpMarker("User can right-click the picker to change mode.");
            ImGuiColorEditFlags flags = misc_flags;
            if (!alpha) { flags |= ImGuiColorEditFlags.NoAlpha; }
            if (alpha_bar) { flags |= ImGuiColorEditFlags.AlphaBar; }
            if (!side_preview) { flags |= ImGuiColorEditFlags.NoSidePreview; }
            if (picker_mode == 1) { flags |= ImGuiColorEditFlags.PickerHueBar; }
            if (picker_mode == 2) { flags |= ImGuiColorEditFlags.PickerHueWheel; }
            if (display_mode == 1) { flags |= ImGuiColorEditFlags.NoInputs; }
            if (display_mode == 2) { flags |= ImGuiColorEditFlags.DisplayRGB; }
            if (display_mode == 3) { flags |= ImGuiColorEditFlags.DisplayHSV; }
            if (display_mode == 4) { flags |= ImGuiColorEditFlags.DisplayHex; }
            ColorPicker4("MyColor##4", ref color_vec4, flags, ref ref_color_v.X); // ref_color ? ref_color_v.X : null);

            Text("Set defaults in code:");
            SameLine(); HelpMarker("SetColorEditOptions() is designed to allow you to set boot-time default.\n" +
                "We don't have Push/Pop functions because you can force options on a per-widget basis if needed," +
                "and the user can change non-forced ones with the options menu.\nWe don't have a getter to avoid" +
                "encouraging you to persistently save values that aren't forward-compatible.");
            if (Button("Default: Uint8 + HSV + Hue Bar"))
            {
                SetColorEditOptions(ImGuiColorEditFlags.Uint8 | ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.PickerHueBar);
            }
            if (Button("Default: Float + HDR + Hue Wheel"))
            {
                SetColorEditOptions(ImGuiColorEditFlags.Float | ImGuiColorEditFlags.HDR | ImGuiColorEditFlags.PickerHueWheel);
            }
            // HSV
            Spacing();
            Text("HSV encoded colors");
            SameLine(); HelpMarker("By default, colors are given to ColorEdit and ColorPicker in RGB, but ImGuiColorEditFlags_InputHSV" +
                "allows you to store colors as HSV and pass them to ColorEdit and ColorPicker as HSV. This comes with the" +
                "added benefit that you can manipulate hue values with the picker even when saturation or value are zero.");
            Text("Color widget with InputHSV:");
            ColorEdit4("HSV shown as RGB##1", ref color_hsv, ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.InputHSV | ImGuiColorEditFlags.Float);
            ColorEdit4("HSV shown as HSV##1", ref color_hsv, ImGuiColorEditFlags.DisplayHSV | ImGuiColorEditFlags.InputHSV | ImGuiColorEditFlags.Float);
            DragFloat4("Raw HSV values", ref color_hsv, 0.01f, 0.0f, 1.0f);

            TreePop();
        }

    }

    private class RangeWidgetsSubsection
    {
        float begin = 10, end = 90;
        int begin_i = 100, end_i = 1000;

        public void Update()
        {
            if (!TreeNode("Range Widgets")) return;

            DragFloatRange2("range float", ref begin, ref end, 0.25f, 0.0f, 100.0f, "Min: %.1f %%", "Max: %.1f %%");
            DragIntRange2("range int", ref begin_i, ref end_i, 5, 0, 1000, "Min: %d units", "Max: %d units");
            DragIntRange2("range int (no bounds)", ref begin_i, ref end_i, 5, 0, 0, "Min: %d units", "Max: %d units");

            TreePop();
        }
    }

    private class MultiComponentWidgetsSubsection
    {
        Vec2 vec2f = new(0.10f, 0.20f);
        Vec3 vec3f = new(0.10f, 0.20f, 0.30f);
        Vec4 vec4f = new(0.10f, 0.20f, 0.30f, 0.44f);
        readonly int[] vec4i = [1, 5, 100, 255];

        public void Update()
        {
            if (!TreeNode("Multi-component Widgets")) return;

            InputFloat2("input float2", ref vec2f);
            DragFloat2("drag float2", ref vec2f, 0.01f, 0.0f, 1.0f);
            SliderFloat2("slider float2", ref vec2f, 0.0f, 1.0f);
            InputInt2("input int2", ref vec4i[0]);
            DragInt2("drag int2", ref vec4i[0], 1, 0, 255);
            SliderInt2("slider int2", ref vec4i[0], 0, 255);
            Spacing();

            InputFloat3("input float3", ref vec3f);
            DragFloat3("drag float3", ref vec3f, 0.01f, 0.0f, 1.0f);
            SliderFloat3("slider float3", ref vec3f, 0.0f, 1.0f);
            InputInt3("input int3", ref vec4i[0]);
            DragInt3("drag int3", ref vec4i[0], 1, 0, 255);
            SliderInt3("slider int3", ref vec4i[0], 0, 255);
            Spacing();

            InputFloat4("input float4", ref vec4f);
            DragFloat4("drag float4", ref vec4f);
            SliderFloat4("slider float4", ref vec4f, 0.0f, 1.0f);
            InputInt4("input int4", ref vec4i[0]);
            DragInt4("drag int4", ref vec4i[0], 1, 0, 255);
            SliderInt4("slider int4", ref vec4i[0], 0, 255);

            TreePop();
        }
    }

    private class VerticalSlidersSubsection
    {
        readonly float spacing = 4;
        int int_value = 0;
        readonly float[] values_vert = [0.0f, 0.60f, 0.35f, 0.9f, 0.70f, 0.20f, 0.0f];
        float col_red = 1.0f;
        float col_green = 1.0f;
        float col_blue = 1.0f;
        readonly float[] values2 = [0.20f, 0.80f, 0.40f, 0.25f];

        public void Update()
        {
            if (!TreeNode("Vertical Sliders")) return;

            PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vec2(spacing, spacing));

            VSliderInt("##int", new Vec2(18, 160), ref int_value, 0, 5);
            SameLine();

            PushID("set1");
            for (int i = 0; i < 7; i++)
            {
                if (i > 0)
                {
                    SameLine();
                }
                PushID(i);

                ColorConvertHSVtoRGB(i / 7.0f, 0.5f, 0.5f, out col_red, out col_green, out col_blue);
                Vec4 col_slider = new(col_red, col_green, col_blue, 1.0f);
                ColorConvertHSVtoRGB(i / 7.0f, 0.6f, 0.5f, out col_red, out col_green, out col_blue);
                Vec4 col_slider_hov = new(col_red, col_green, col_blue, 1.0f);
                ColorConvertHSVtoRGB(i / 7.0f, 0.7f, 0.5f, out col_red, out col_green, out col_blue);
                Vec4 col_slider_act = new(col_red, col_green, col_blue, 1.0f);
                ColorConvertHSVtoRGB(i / 7.0f, 0.9f, 0.9f, out col_red, out col_green, out col_blue);
                Vec4 col_slider_grab = new(col_red, col_green, col_blue, 1.0f);

                PushStyleColor(ImGuiCol.FrameBg, col_slider);
                PushStyleColor(ImGuiCol.FrameBgHovered, col_slider_hov);
                PushStyleColor(ImGuiCol.FrameBgActive, col_slider_act);
                PushStyleColor(ImGuiCol.SliderGrab, col_slider_grab);
                VSliderFloat("##v", new Vec2(18, 160), ref values_vert[i], 0.0f, 1.0f, "");
                if (IsAnyItemActive() || IsItemHovered())
                {
                    SetTooltip(values_vert[i].ToString());
                }
                PopStyleColor(4);
                PopID();
            }
            PopID();

            SameLine();
            PushID("set2");

            int rows = 3;
            Vec2 small_slider_size = new(18, 50);//(float)(int)((160.0f - (rows - 1) * spacing / rows)));
            for (int nx = 0; nx < 4; nx++)
            {
                if (nx > 0) { SameLine(); }
                BeginGroup();
                for (int ny = 0; ny < rows; ny++)
                {
                    PushID(nx * rows + ny);
                    VSliderFloat("##v", small_slider_size, ref values2[nx], 0.0f, 1.0f, "");
                    if (IsItemActive() || IsItemHovered())
                    {
                        SetTooltip(values2[nx].ToString());
                    }
                    PopID();
                }
                EndGroup();
            }
            PopID();

            SameLine();
            PushID("set3");
            for (int i = 0; i < 4; i++)
            {
                if (i > 0)
                {
                    SameLine();
                }
                PushID(i);
                PushStyleVar(ImGuiStyleVar.GrabMinSize, 40);
                VSliderFloat("##v", new Vec2(40, 160), ref values_vert[i], 0.0f, 1.0f, "%.2f\nsec");
                PopStyleVar();
                PopID();
            }
            PopID();
            PopStyleVar();
            TreePop();
        }
    }
}
