using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SCDearImGui.MonoGame.Demos.GuiElements.Concepts;
using SCDearImGui.MonoGame.Demos.GuiElements.DemoWindow;
using SCDearImGui.MonoGame.Demos.GuiElements.MiniApps;
using System.IO;

namespace SCDearImGui.MonoGame.Demos;

public class Program : Game
{
    // The GUI renderer - this is responsible for drawing the GUI
    private readonly ImGuiRenderer guiRenderer;

    // Main demo window
    private readonly DemoWindow demoWindow;

    // Concept demos
    private readonly MainMenuBar mainMenuBar = new();
    private readonly AutoResizeWindow autoResizeWindow = new();
    private readonly ConstrainedResizeWindow constrainedResizeWindow = new();
    private readonly TitleManipulationWindows titleManipulationWindow = new();
    private readonly CustomRenderingWindow customRenderingWindow = new();
    private readonly LongTextDisplayWindow longTextDisplayWindow = new();

    // Mini app demos
    private readonly DisplaySettingsWindow displaySettingsWindow;
    private readonly ModelAndControls modelAndControls;
    private readonly ModelViewerWindow modelViewerWindow;
    private readonly LogWindow logWindow = new(new ExampleLogWindowContentSource(), maxEntryCount: 1000);
    private readonly ConsoleWindow consoleWindow = new(new ExampleConsole());
    private readonly DocumentsWindow documentsWindow = new(new ExampleDocumentStore());
    private readonly AssetsBrowserWindow assetsBrowserWindow = new();
    private readonly PropertyEditorWindow propertyEditorWindow = new();
    private readonly SimpleOverlay simpleOverlay = new();
    private readonly SimpleLayoutWindow simpleLayoutWindow = new();
    private readonly SimpleFullscreenWindow simpleFullscreenWindow = new();

    // Flags for showing native ImGui demos & tools
    private bool showImGuiNativeDemoWindow = false;
    private bool showImGuiStyleEditor = false;
    private bool showImGuiMetricsWindow = false;
    private bool showImGuiAboutWindow = false;

    private Program()
    {
        // First, general MonoGame startup stuff:
        Window.Title = "MonoGame & ImGui.NET";
        Window.AllowUserResizing = true;
        IsMouseVisible = true;
        Content.RootDirectory = "Content";

        var graphicsDeviceManager = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
            PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
            IsFullScreen = true
        };
        graphicsDeviceManager.ApplyChanges();

        // Instantiate the GUI renderer. This is responsible for drawing the GUI:
        guiRenderer = new ImGuiRenderer(this);

        // We have classes encapsulating each of our individual demos. While most of them don't
        // have any dependency on the game itself (and we can thus use inline field initialisers for them - see above),
        // a few do, so we create them here, passing along what they need.
        //
        // Note that this includes the main demo window, which we don't want to give hard-coded knowledge of the
        // other windows, but we do want it to include menu items for opening and closing them. So, we provide it with
        // "MenuItem" objects that include what is essentially a callback to handle being selected and unselected.
        displaySettingsWindow = new(Window, graphicsDeviceManager, guiRenderer);
        modelAndControls = new(GraphicsDevice, Content, "Models/suzanne");
        modelViewerWindow = new(GraphicsDevice, Content, guiRenderer, "Models/suzanne");
        demoWindow = new(this)
        {
            ExamplesMenuSections =
            {
                new("Concepts")
                {
                    new("Main menu bar", () => mainMenuBar.IsVisible),
                    new("Long text display", () => longTextDisplayWindow.IsOpen),
                    new("Automatic resizing", () => autoResizeWindow.IsOpen),
                    new("Constrained resizing", () => constrainedResizeWindow.IsOpen),
                    new("Manipulating window titles", () => titleManipulationWindow.AreOpen),
                    new("Custom rendering", () => customRenderingWindow.IsOpen),
                },
                new("Mini Apps")
                {
                    new("Log", () => logWindow.IsOpen),
                    new("Console", () => consoleWindow.IsOpen),
                    new("Model viewer", () => modelViewerWindow.IsOpen),
                    new("Model and controls", () => modelAndControls.IsVisible),
                    new("Assets browser", () => assetsBrowserWindow.IsOpen),
                    new("Property editor", () => propertyEditorWindow.IsOpen),
                    new("Documents", () => documentsWindow.IsOpen),
                    new("Display settings control", () => displaySettingsWindow.IsOpen),
                    new("Simple layout", () => simpleLayoutWindow.IsOpen),
                    new("Simple overlay", () => simpleOverlay.IsVisible),
                    new("Simple fullscreen window", () => simpleFullscreenWindow.IsOpen),
                },
                new("Native")
                {
                    new("Native Dear ImGui Demo Window", () => showImGuiNativeDemoWindow),
                },
            },
            ToolsMenuSections =
            {
                new("Native")
                {
                    new("Metrics/Debugger", () => showImGuiMetricsWindow),
                    new("Style Editor", () => showImGuiStyleEditor),
                    new("About Dear ImGui", () => showImGuiAboutWindow),
                }
            }
        };
    }

    /// <summary>
    /// The program entry point.
    /// </summary>
    public static void Main()
    {
        using var game = new Program();
        game.Run();
    }

    /// <inheritdoc />
    protected override void LoadContent()
    {
        // Load the GUI content - specifically, the fonts.
        ImGui.GetIO().Fonts.AddFontDefault();
        //guiRenderer.RegisterFont("Content\\Fonts\\Roboto-Regular.ttf", 24);
        guiRenderer.RegisterFont(File.ReadAllBytes("Content\\Fonts\\Roboto-Regular.ttf"), 24);
        guiRenderer.ApplyStyleAndFonts();

        // A couple of our demo windows use content, too, so tell them to load what they need:
        modelAndControls.LoadContent();
        modelViewerWindow.LoadContent();
    }

    /// <inheritdoc />
    protected override void UnloadContent()
    {
        ImGui.GetIO().Fonts.Clear();

        modelAndControls.UnloadContent();
        modelViewerWindow.UnloadContent();
    }

    /// <inheritdoc />
    // NB: no need for base.Update(..) in here, since we know that we haven't added any components to update.
    protected override void Update(GameTime gameTime)
    {
        // Display settings window can make (GUI scale) changes that need to happen outside of an ImGui frame:
        displaySettingsWindow.PreUpdate();

        // BeginUpdate needs to be called every update before submitting anything to ImGui:
        guiRenderer.BeginUpdate(gameTime);

        // Now tell all our demos to update themselves
        // (which will make submissions to ImGui & update their state in response to GUI interactions):
        mainMenuBar.Update();
        autoResizeWindow.Update();
        constrainedResizeWindow.Update();
        titleManipulationWindow.Update();
        customRenderingWindow.Update();
        longTextDisplayWindow.Update();

        modelAndControls.Update();
        modelViewerWindow.Update();
        assetsBrowserWindow.Update();
        propertyEditorWindow.Update();
        simpleOverlay.Update(gameTime);
        consoleWindow.Update();
        logWindow.Update();
        displaySettingsWindow.Update();
        simpleLayoutWindow.Update();
        documentsWindow.Update();
        simpleFullscreenWindow.Update();

        demoWindow.Update();

        // Also submit the native ImGui tools if we've been told to do so:
        if (showImGuiNativeDemoWindow)
        {
            ImGui.ShowDemoWindow(ref showImGuiNativeDemoWindow);
        }

        if (showImGuiStyleEditor)
        {
            ImGui.Begin("Dear ImGui Style Editor", ref showImGuiStyleEditor);
            ImGui.ShowStyleEditor();
            ImGui.End();
        }

        if (showImGuiMetricsWindow)
        {
            ImGui.ShowMetricsWindow(ref showImGuiMetricsWindow);
        }

        if (showImGuiAboutWindow)
        {
            ImGui.ShowAboutWindow(ref showImGuiAboutWindow);
        }

        // EndUpdate needs to be called every update once all ImGui submissions have been made:
        guiRenderer.EndUpdate();
    }

    /// <inheritdoc />
    // NB: no need for base.Draw(..) in here, since we know that we haven't added any components to draw.
    protected override void Draw(GameTime gameTime)
    {
        // A couple of our demos have stuff to draw other than the GUI, so have their own draw methods.

        // This one goes right at the start because it changes the render target (it draws to a texture), which
        // also clears graphics device state - so putting it after anything else would overwrite anything they've done:
        modelViewerWindow.DrawModelToTexture();

        GraphicsDevice.Clear(Color.CornflowerBlue); // Clear the graphics device and give ourselves a nice blue background
        modelAndControls.DrawModel(); // Draw the model part of the "model and controls" demo
        guiRenderer.Draw(); // ..and of course draw the GUI
    }
}
