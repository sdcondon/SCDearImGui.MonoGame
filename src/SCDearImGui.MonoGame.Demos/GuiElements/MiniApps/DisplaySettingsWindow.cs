using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Linq;
using static ImGuiNET.ImGui;

namespace SCDearImGui.MonoGame.Demos.GuiElements.MiniApps;

class DisplaySettingsWindow
{
    private static readonly TimeSpan UiScaleDebounceDuration = TimeSpan.FromSeconds(0.5);

    public bool IsOpen;

    private readonly GameWindow window;
    private readonly GraphicsDeviceManager graphicsDeviceManager;
    private readonly DisplayMode[] displayModes;
    private readonly string[] displayModeDescriptions;
    private readonly ImGuiRenderer guiRenderer;
    private readonly Stopwatch guiScaleDebouncer = new();

    private int displayModeIndex;
    private bool isFullScreen;
    private float uiScale = 1f;

    public DisplaySettingsWindow(GameWindow window, GraphicsDeviceManager graphicsDeviceManager, ImGuiRenderer guiRenderer, bool isOpen = false)
    {
        this.IsOpen = isOpen;

        this.window = window;
        this.graphicsDeviceManager = graphicsDeviceManager;
        this.guiRenderer = guiRenderer;
        this.displayModes = [.. graphicsDeviceManager.GraphicsDevice.Adapter.SupportedDisplayModes];
        this.displayModeDescriptions = [.. displayModes.Select(a => $"{a.Width}x{a.Height} ({a.AspectRatio:F2}:1)")];

        this.displayModeIndex = Array.IndexOf(displayModes, graphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode);
        this.isFullScreen = graphicsDeviceManager.IsFullScreen;
    }

    public void PreUpdate()
    {
        if (guiScaleDebouncer.IsRunning && guiScaleDebouncer.Elapsed > UiScaleDebounceDuration)
        {
            guiRenderer.ApplyStyleAndFonts(uiScale);
            guiScaleDebouncer.Reset();
        }
    }

    public void Update()
    {
        if (!IsOpen) return;

        SetNextWindowSize(new(300, 220), ImGuiCond.FirstUseEver);
        SetNextWindowPos(new(10, 140), ImGuiCond.FirstUseEver);

        if (!Begin("Example: Display Settings", ref IsOpen, ImGuiWindowFlags.NoCollapse))
        {
            End();
            return;
        }

        if (Combo("Window Size", ref displayModeIndex, displayModeDescriptions, displayModeDescriptions.Length) && displayModeIndex > -1)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = displayModes[displayModeIndex].Width;
            graphicsDeviceManager.PreferredBackBufferHeight = displayModes[displayModeIndex].Height;
            graphicsDeviceManager.PreferredBackBufferFormat = displayModes[displayModeIndex].Format;
            graphicsDeviceManager.ApplyChanges();
        }

        if (Checkbox("Fullscreen", ref isFullScreen))
        {
            graphicsDeviceManager.IsFullScreen = isFullScreen;
            graphicsDeviceManager.ApplyChanges();
        }

        if (SliderFloat("UI Scale", ref uiScale, 0.5f, 2.0f, "%.2f"))
        {
            // Scaling the UI involves reloading fonts. We don't want to do this
            // *too* much (e.g. for every tick along the slider), so we debounce.
            // That is, we use a timer to make sure we only actually change the scale
            // (in PreUpdate - has to happen outside of a frame) if the slider value
            // hasn't changed for a couple of seconds.
            guiScaleDebouncer.Restart();
        }

        if (CollapsingHeader("Display Properties"))
        {
            Text($"Window Screen Device Name: {window.ScreenDeviceName}");
            Text($"Window Position X,Y: {window.Position.X},{window.Position.Y}");
            Text($"Window ClientBounds L - R: {window.ClientBounds.Left} - {window.ClientBounds.Right}");
            Text($"Window ClientBounds T - B: {window.ClientBounds.Top} - {window.ClientBounds.Bottom}");
            Text($"GDM Preferred Back Buffer WxH: {graphicsDeviceManager.PreferredBackBufferWidth}x{graphicsDeviceManager.PreferredBackBufferHeight}");
            Text($"GD Adapter Desc: {graphicsDeviceManager.GraphicsDevice.Adapter.Description}");
            Text($"GD Display Mode WxH: {graphicsDeviceManager.GraphicsDevice.DisplayMode.Width}x{graphicsDeviceManager.GraphicsDevice.DisplayMode.Height}");
            Text($"GD Presentation Parameters Back Buffer WxH: {graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferWidth}x{graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferHeight}");
            Text($"GD Viewport X,Y: {graphicsDeviceManager.GraphicsDevice.Viewport.X},{graphicsDeviceManager.GraphicsDevice.Viewport.Y}");
            Text($"GD Viewport WxH: {graphicsDeviceManager.GraphicsDevice.Viewport.Width}x{graphicsDeviceManager.GraphicsDevice.Viewport.Height}");
            Text($"ImGui Window Size X,Y: {GetWindowViewport().Size.X},{GetWindowViewport().Size.Y}");
        }

        Separator();

        var speed = guiRenderer.MouseWheelVScale;
        if (SliderFloat("GUI Mouse Wheel V Scaling", ref speed, 1f / 400, 1f / 40))
        {
            guiRenderer.MouseWheelVScale = speed;
        }

        speed = guiRenderer.MouseWheelHScale;
        if (SliderFloat("GUI Mouse Wheel H Scaling", ref speed, 1f / 400, 1f / 40))
        {
            guiRenderer.MouseWheelHScale = speed;
        }

        End();
    }
}
