using ImGuiNET;
using Microsoft.Xna.Framework;
using static ImGuiNET.ImGui;
using static SCDearImGui.MonoGame.Demos.GuiElements.GuiElementHelpers;

namespace SCDearImGui.MonoGame.Demos.GuiElements.MiniApps;

class SimpleOverlay(bool isVisible = false)
{
    private static Vector2 Margin = new(10f, 10f);

    public bool IsVisible = isVisible;

    private Corner corner = 0;

    public void Update(GameTime gameTime)
    {
        if (!IsVisible) return;

        // NB: we use workarea not full viewport, so that we respect
        // any present menu bar etc when positioning.
        System.Numerics.Vector2 windowPosition = new();
        System.Numerics.Vector2 windowPivot = new();
        if (corner == Corner.TopLeft || corner == Corner.BottomLeft)
        {
            windowPosition.X = GetMainViewport().WorkPos.X + Margin.X;
            windowPivot.X = 0f;
        }
        else
        {
            windowPosition.X = GetMainViewport().WorkSize.X - Margin.X;
            windowPivot.X = 1f;
        }

        if (corner == Corner.TopLeft || corner == Corner.TopRight)
        {
            windowPosition.Y = GetMainViewport().WorkPos.Y + Margin.Y;
            windowPivot.Y = 0f;
        }
        else
        {
            windowPosition.Y = GetMainViewport().WorkSize.Y - Margin.Y;
            windowPivot.Y = 1f;
        }

        SetNextWindowPos(windowPosition, ImGuiCond.Always, windowPivot);
        SetNextWindowBgAlpha(0.35f);

        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.AlwaysAutoResize
            | ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoFocusOnAppearing
            | ImGuiWindowFlags.NoMove;

        if (Begin("Example: Simple overlay", windowFlags))
        {
            Text("Simple overlay\nin the corner of the screen\n(right-click to change position)");
            Separator();
            if (IsMousePosValid())
            {
                ImGuiIOPtr io = GetIO();
                Text(string.Format("Mouse Position: ({0},{1})", io.MousePos.X, io.MousePos.Y));
            }
            else
            {
                Text("Mouse Position: <invalid>");
            }

            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            Text($"Frames per second: {frameRate:F2}");

            if (BeginPopupContextWindow())
            {
                //if (ImGui.MenuItem("Custom", null, corner == -1)) { corner = -1; }
                if (MenuItem("Top-left", null, corner == Corner.TopLeft)) { corner = Corner.TopLeft; }
                if (MenuItem("Top-right", null, corner == Corner.TopRight)) { corner = Corner.TopRight; }
                if (MenuItem("Bottom-left", null, corner == Corner.BottomLeft)) { corner = Corner.BottomLeft; }
                if (MenuItem("Bottom-right", null, corner == Corner.BottomRight)) { corner = Corner.BottomRight; }

                EndPopup();
            }
        }

        End();
    }

    private enum Corner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
