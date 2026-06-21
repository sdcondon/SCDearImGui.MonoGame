using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SCDearImGui.MonoGame;

/// <summary>
/// Renderer for Dear ImGui.
/// </summary>
public sealed class ImGuiRenderer : IDisposable
{
    private const float MOUSE_WHEEL_DELTA = 120;
    private const int INITIAL_BUFFER_SIZE = 512;

    private static readonly int ImDrawVertexStride = Marshal.SizeOf<ImDrawVert>();

    private static readonly VertexDeclaration ImDrawVertexDeclaration = new(
        ImDrawVertexStride,
        new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
        new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0));

    // Context
    private readonly Game _game;
    private readonly nint _imGuiContext;
    private readonly ImGuiIOPtr _imGuiIO;
    private readonly nint _iniFilePathPtr;
    private readonly IInputCaptureState? _inputCaptureState;

    // Graphics
    private readonly GraphicsDevice _graphicsDevice;
    private readonly RasterizerState _rasterizerState;
    private readonly BasicEffect _effect;
    private readonly Dictionary<nint, Texture2D> _texturesById;

    private readonly List<ImGuiFontRegistration> fontRegistrations = [];

    private byte[] _vertexData = new byte[INITIAL_BUFFER_SIZE * ImDrawVertexStride];
    private VertexBuffer _vertexBuffer;
    private byte[] _indexData = new byte[INITIAL_BUFFER_SIZE * sizeof(ushort)];
    private IndexBuffer _indexBuffer;

    private ImGuiStyle referenceStyle;
    private float currentUiScale = 0;
    private nint? _fontAtlasTextureId;

    private nint _nextTextureId;

    // Input
    private readonly List<TextInputEventArgs> _textInputs = new(2);
    private KeyboardState _lastKeyboardState;
    private MouseState _lastMouseState;

    /// <summary>
    /// Initialises a new instance of the <see cref="ImGuiRenderer"/> class.
    /// </summary>
    /// <param name="game">
    /// The <see cref="Game"/> that this renderer is to be used by.
    /// </param>
    /// <param name="inputCaptureState">
    /// <para>
    /// Optional. A representation of input capture state to be used by the renderer.
    /// </para>
    /// <para>
    /// The renderer will not consume input if the capture state indicates that it has already been captured
    /// when <see cref="BeginUpdate"/> is invoked. The renderer will update this object appropriately during
    /// <see cref="EndUpdate"/>, and whenever <see cref="UpdateInputCaptureState"/> is called.
    /// </para>
    /// </param>
    /// <param name="iniFilePath">
    /// The name of the ini file to use - or null to use no ini file (meaning no GUI state persistence will occur).
    /// </param>
    public ImGuiRenderer(Game game, IInputCaptureState? inputCaptureState = null, string? iniFilePath = null)
    {
        // Setup context
        _game = game ?? throw new ArgumentNullException(nameof(game));

        _imGuiContext = ImGui.CreateContext();
        ImGui.SetCurrentContext(_imGuiContext);
        _imGuiIO = ImGui.GetIO();

        // Set the ini file path
        unsafe
        {
            _iniFilePathPtr = Marshal.StringToHGlobalAnsi(iniFilePath);
            _imGuiIO.NativePtr->IniFilename = (byte*)_iniFilePathPtr;
        }

        // Set the input filter
        _inputCaptureState = inputCaptureState;

        // Store reference style so end user doesn't *have* to:
        StoreReferenceStyle();

        // Setup graphics
        _graphicsDevice = game.GraphicsDevice;
        _rasterizerState = new()
        {
            CullMode = CullMode.None,
            DepthBias = 0,
            FillMode = FillMode.Solid,
            MultiSampleAntiAlias = false,
            ScissorTestEnable = true,
            SlopeScaleDepthBias = 0
        };
        _effect = new BasicEffect(_graphicsDevice)
        {
            World = Matrix.Identity,
            View = Matrix.Identity,
            TextureEnabled = true,
            VertexColorEnabled = true,
        };

        _texturesById = [];
        _vertexBuffer = new VertexBuffer(
            _graphicsDevice,
            ImDrawVertexDeclaration,
            INITIAL_BUFFER_SIZE,
            BufferUsage.None);
        _indexBuffer = new IndexBuffer(
            _graphicsDevice,
            IndexElementSize.SixteenBits,
            INITIAL_BUFFER_SIZE,
            BufferUsage.None);

        // Setup input
        _game.Window.TextInput += HandleWindowTextInput;
    }

    ~ImGuiRenderer()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets or sets the category to use for trace messages. Defaults to the full name of the <see cref="ImGuiRenderer"/> type.
    /// </summary>
    public string TraceCategory { get; set; } = typeof(ImGuiRenderer).FullName!;

    /// <summary>
    /// Gets the scale that was provided on the last invocation of <see cref="ApplyStyleAndFonts"/>.
    /// Throws if it has not yet been invoked.
    /// </summary>
    public float Scale
    {
        get
        {
            if (currentUiScale == 0)
            {
                throw new InvalidOperationException("Cannot retrieve UI scale - it has not yet been set.");
            }

            return currentUiScale;
        }
    }

    /// <summary>
    /// <para>
    /// Sets up ImGui for a new frame.
    /// </para>
    /// <para>
    /// Should be called in your Update method, prior to any <see cref="ImGui"/> calls.
    /// </para>
    /// </summary>
    public void BeginUpdate(GameTime gameTime)
    {
        ImGui.SetCurrentContext(_imGuiContext);
        UpdateIO(gameTime);
        ImGui.NewFrame();
    }

    /// <summary>
    /// <para>
    /// Tells ImGui that all GUI submissions have been made for the current frame.
    /// </para>
    /// <para>
    /// Should be called in your Update method, after all <see cref="ImGui"/> calls.
    /// </para>
    /// </summary>
    public void EndUpdate()
    {
        UpdateInputCaptureState();
        ImGui.SetCurrentContext(_imGuiContext);
        ImGui.EndFrame();
    }

    /// <summary>
    /// <para>
    /// If the renderer has been given an <see cref="IInputCaptureState"/> object, prompts it to
    /// immediately check whether the GUI wants to capture mouse or keyboard input, and update it
    /// appropriately.
    /// </para>
    /// <para>
    /// This is automatically done during <see cref="EndUpdate"/> but, depending on the 
    /// structure of your update logic, you may want or need to also trigger it earlier, when
    /// only some of your GUI elements have been submitted.
    /// </para>
    /// </summary>
    public void UpdateInputCaptureState()
    {
        if (_inputCaptureState == null)
        {
            return;
        }

        if (_imGuiIO.WantCaptureMouse)
        {
            _inputCaptureState.IsMouseCaptured = true;
        }

        if (_imGuiIO.WantCaptureKeyboard)
        {
            _inputCaptureState.IsKeyboardCaptured = true;
        }
    }

    /// <summary>
    /// <para>
    /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline.
    /// </para>
    /// <para>
    /// Should be called in your Draw method.
    /// </para>
    /// </summary>
    public void Draw()
    {
        ImGui.SetCurrentContext(_imGuiContext);

        ImGui.Render();
        var drawData = ImGui.GetDrawData();

        // Store graphics device state for restoration after we're done
        var lastRasterizer = _graphicsDevice.RasterizerState;
        var lastDepthStencil = _graphicsDevice.DepthStencilState;
        var lastBlendFactor = _graphicsDevice.BlendFactor;
        var lastBlendState = _graphicsDevice.BlendState;
        var lastScissorBox = _graphicsDevice.ScissorRectangle;
        var lastViewport = _graphicsDevice.Viewport;

        SetGraphicsDeviceState(drawData);
        SetBufferData(drawData);
        RenderCommandLists(drawData);

        // Restore graphics device state
        _graphicsDevice.RasterizerState = lastRasterizer;
        _graphicsDevice.DepthStencilState = lastDepthStencil;
        _graphicsDevice.BlendFactor = lastBlendFactor;
        _graphicsDevice.BlendState = lastBlendState;
        _graphicsDevice.ScissorRectangle = lastScissorBox;
        _graphicsDevice.Viewport = lastViewport;
    }

    /// <summary>
    /// <para>
    /// Creates an identifier for a texture, which can then be passed to ImGui calls such as <see cref="ImGui.Image" />.
    /// </para>
    /// <para>
    /// NB: The renderer considers itself as taking ownership of the lifetime of the passed texture when this method is called - it will be disposed when unregistered.
    /// </para>
    /// </summary>
    public nint RegisterTexture(Texture2D texture)
    {
        var id = _nextTextureId++;
        _texturesById.Add(id, texture);
        return id;
    }

    /// <summary>
    /// Removes a previously created texture identifier, releasing its reference and disposing the texture object.
    /// </summary>
    /// <param name="textureId">The ID of the texture to unregister</param>
    /// <returns>True if the texture identifier was valid and a texture was unregistered. Otherwise false.</returns>
    public bool UnregisterTexture(nint textureId)
    {
        bool textureRemoved;
        if (textureRemoved = _texturesById.TryGetValue(textureId, out var texture))
        {
            _texturesById.Remove(textureId);
            texture?.Dispose();
        }

        return textureRemoved;
    }

    /// <summary>
    /// Register a font that will be (re-)loaded whenever <see cref="ApplyStyleAndFonts"/> is invoked.
    /// </summary>
    public ImGuiFontRegistration RegisterFont(string ttfFilePath, float defaultSizePixels)
    {
        ImGuiFontRegistration fontRegistration = new(ttfFilePath, defaultSizePixels);
        fontRegistrations.Add(fontRegistration);
        return fontRegistration;
    }

    /// <summary>
    /// Register a font that will be (re-)loaded whenever <see cref="ApplyStyleAndFonts"/> is invoked.
    /// </summary>
    public ImGuiFontRegistration RegisterFont(byte[] ttfData, float defaultSizePixels)
    {
        ImGuiFontRegistration fontRegistration = new(ttfData, defaultSizePixels);
        fontRegistrations.Add(fontRegistration);
        return fontRegistration;
    }

    /// <summary>
    /// <para>
    /// Stores the current style as the reference style that is applied
    /// when <see cref="ApplyStyleAndFonts"/> is invoked.
    /// </para>
    /// <para>
    /// We use the approach of requiring a reference style because incrementally updating 
    /// sizes with e.g. ImGui.GetStyle().ScaleAllSizes(newScale/oldScale) doesn't work
    /// very well at all. Presumably because rounding errors add up pretty quickly.
    /// </para>
    /// </summary>
    public void StoreReferenceStyle()
    {
        unsafe
        {
            referenceStyle = *ImGui.GetStyle().NativePtr;
        }
    }

    /// <summary>
    /// <para>
    /// Applies the style stored with <see cref="StoreReferenceStyle"/> (or Dear ImGui's default style if this hasn't been invoked),
    /// with sizings scaled by a given amount. Loads all registered fonts, with sizes also scaled. Finally, rebuilds the font atlas.
    /// </para>
    /// <list type="bullet">
    /// <item>NB #1: Needs to be called between drawing previous frame and starting new one, or ImGui will complain.</item>
    /// <item>NB #2: Will clobber any font not registered with <see cref="RegisterFont"/>. Can't see an easy way to reload only a subset of fonts - looks like they can only be cleared en masse. Which is annoying.</item>
    /// <item>NB #3: Fairly expensive because it reloads all fonts. Don't call me too often - consider debouncing if necessary (see the display settings window in the demo project for an example of this).</item>
    /// </list>
    /// </summary>
    public void ApplyStyleAndFonts(float scale = 1f)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scale, 0);

        ImGui.SetCurrentContext(_imGuiContext);

        unsafe
        {
            // Copy in reference style
            *ImGui.GetStyle().NativePtr = referenceStyle;
        }

        // Scale all sizes in the style
        ImGui.GetStyle().ScaleAllSizes(scale);

        var fontReloadStopwatch = Stopwatch.StartNew();

        // Replace fonts to atlas
        // todo-robustness: clobbers all fonts added directly rather than through registerfont. check for this and throw?
        _imGuiIO.Fonts.Clear();
        foreach (var r in fontRegistrations)
        {
            r.AddToAtlas(_imGuiIO.Fonts, scale);
        }

        // If the font atlas has already been built, unregister the registered texture first (which also disposes the XNA texture object).
        if (_fontAtlasTextureId.HasValue)
        {
            UnregisterTexture(_fontAtlasTextureId.Value);
        }

        // Get font texture data from ImGui..
        _imGuiIO.Fonts.GetTexDataAsRGBA32(out nint atlasPointer, out int width, out int height, out int bytesPerPixel);
        byte[] atlasPixels = new byte[width * height * bytesPerPixel];
        Marshal.Copy(atlasPointer, atlasPixels, 0, atlasPixels.Length);
        _imGuiIO.Fonts.ClearTexData(); // Don't forget to tidy up ImGui texdata once we've copied it out

        // ..and copy it to an XNA texture object:
        Texture2D atlasTexture = new(_graphicsDevice, width, height, false, SurfaceFormat.Color);
        atlasTexture.SetData(atlasPixels);

        // Register the texture for use (so that our RenderCommandLists method can recognise it
        // and bind the font atlas texture in response), then tell ImGui to use the registered
        // ID in its commands to render text: 
        _fontAtlasTextureId = RegisterTexture(atlasTexture);
        _imGuiIO.Fonts.SetTexID(_fontAtlasTextureId.Value);

        // Store scale for querying by consumers:
        currentUiScale = scale;

        Trace.Write($"{_imGuiIO.Fonts.Fonts.Size} font variants loaded in {fontReloadStopwatch.ElapsedMilliseconds}ms.", TraceCategory);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void HandleWindowTextInput(object? sender, TextInputEventArgs eventArgs)
    {
        // NB: This event gets raised on the main game update thread - so never concurrently
        // with any other update logic. However, we still need to queue it up for processing
        // during BeginUpdate rather than handle it directly, because of our input capture logic.
        //
        // If we checked *here* whether keyboard input was already captured, we could (depending on
        // exactly when the hosting app resets capture state) easily be seeing the value from the previous
        // frame (these events generally get fired before the main game Tick), and as such might be blocked
        // by ourself from the previous frame! So, we just queue it up, ultimately making sure that we check
        // capture state in just *one place* per frame, providing predictable behaviour in the face of
        // whatever decision the hosting app wants to make regarding when it resets capture state.
        if (eventArgs.Character == '\t')
        {
            return;
        }

        _textInputs.Add(eventArgs);
    }

    private void UpdateIO(GameTime gameTime)
    {
        if (!_game.IsActive) return;

        _imGuiIO.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _imGuiIO.DisplaySize = new(_graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);
        _imGuiIO.DisplayFramebufferScale = new(1f, 1f);

        var mouseState = Mouse.GetState();
        if (_inputCaptureState?.IsMouseCaptured != true)
        {
            AddMousePosEvent();
            AddMouseWheelEvent();
            AddMouseButtonEvent(0, _lastMouseState.LeftButton, mouseState.LeftButton);
            AddMouseButtonEvent(1, _lastMouseState.RightButton, mouseState.RightButton);
            AddMouseButtonEvent(2, _lastMouseState.MiddleButton, mouseState.MiddleButton);
            AddMouseButtonEvent(3, _lastMouseState.XButton1, mouseState.XButton1);
            AddMouseButtonEvent(4, _lastMouseState.XButton2, mouseState.XButton2);
        }
        _lastMouseState = mouseState;

        var keyboardState = Keyboard.GetState();
        if (_inputCaptureState?.IsKeyboardCaptured != true)
        {
            AddKeyEvents(_lastKeyboardState, keyboardState, false);
            AddKeyEvents(keyboardState, _lastKeyboardState, true);

            foreach (var textInput in _textInputs)
            {
                _imGuiIO.AddInputCharacter(textInput.Character);
            }
        }
        _lastKeyboardState = keyboardState;
        _textInputs.Clear();

        void AddMousePosEvent()
        {
            if (mouseState.X != _lastMouseState.X || mouseState.Y != _lastMouseState.Y)
            {
                _imGuiIO.AddMousePosEvent(mouseState.X, mouseState.Y);
            }
        }

        void AddMouseWheelEvent()
        {
            var scrollDelta = mouseState.ScrollWheelValue - _lastMouseState.ScrollWheelValue;
            var horizontalScrollDelta = mouseState.HorizontalScrollWheelValue - _lastMouseState.HorizontalScrollWheelValue;

            if (scrollDelta != 0 || horizontalScrollDelta != 0)
            {
                _imGuiIO.AddMouseWheelEvent(horizontalScrollDelta / MOUSE_WHEEL_DELTA, scrollDelta / MOUSE_WHEEL_DELTA);
            }
        }

        void AddMouseButtonEvent(int button, ButtonState lastState, ButtonState thisState)
        {
            if (lastState != thisState)
            {
                _imGuiIO.AddMouseButtonEvent(button, thisState == ButtonState.Pressed);
            }
        }

        void AddKeyEvents(KeyboardState fromState, KeyboardState toState, bool isBackwards)
        {
            foreach (var toPressedKey in toState.GetPressedKeys())
            {
                if (fromState[toPressedKey] == KeyState.Up && TryMapKey(toPressedKey, out ImGuiKey imguikey))
                {
                    _imGuiIO.AddKeyEvent(imguikey, !isBackwards);
                }
            }
        }

        static bool TryMapKey(Keys key, out ImGuiKey imGuiKey)
        {
            ImGuiKey? mappedKey = key.TryToImGuiKey();

            if (mappedKey.HasValue)
            {
                imGuiKey = mappedKey.Value;
                return true;
            }
            else
            {
                imGuiKey = ImGuiKey.None;
                return false;
            }
        }
    }

    private void SetGraphicsDeviceState(ImDrawDataPtr drawData)
    {
        // Set render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
        _graphicsDevice.RasterizerState = _rasterizerState;
        _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        _graphicsDevice.BlendFactor = Color.White;
        _graphicsDevice.BlendState = BlendState.NonPremultiplied;

        // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
        drawData.ScaleClipRects(_imGuiIO.DisplayFramebufferScale);

        // Set viewport
        _graphicsDevice.Viewport = new Viewport(
            0,
            0,
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight);
    }

    private void SetBufferData(ImDrawDataPtr drawData)
    {
        if (drawData.TotalVtxCount == 0)
        {
            return;
        }

        // Expand buffers if we need more room
        if (drawData.TotalVtxCount > _vertexBuffer.VertexCount)
        {
            _vertexBuffer.Dispose();

            var newVertexCount = (int)(drawData.TotalVtxCount * 1.5f);
            _vertexBuffer = new VertexBuffer(_graphicsDevice, ImDrawVertexDeclaration, newVertexCount, BufferUsage.None);
            _vertexData = new byte[newVertexCount * ImDrawVertexStride];
        }

        if (drawData.TotalIdxCount > _indexBuffer.IndexCount)
        {
            _indexBuffer.Dispose();

            var newIndexCount = (int)(drawData.TotalIdxCount * 1.5f);
            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, newIndexCount, BufferUsage.None);
            _indexData = new byte[newIndexCount * sizeof(ushort)];
        }

        // Copy ImGui's vertices and indices to a set of managed byte arrays
        int vtxOffset = 0;
        int idxOffset = 0;

        for (var cmdListIx = 0; cmdListIx < drawData.CmdListsCount; cmdListIx++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[cmdListIx];

            Marshal.Copy(cmdList.VtxBuffer.Data, _vertexData, vtxOffset * ImDrawVertexStride, cmdList.VtxBuffer.Size * ImDrawVertexStride);
            Marshal.Copy(cmdList.IdxBuffer.Data, _indexData, idxOffset * sizeof(ushort), cmdList.IdxBuffer.Size * sizeof(ushort));

            vtxOffset += cmdList.VtxBuffer.Size;
            idxOffset += cmdList.IdxBuffer.Size;
        }

        // Copy the managed byte arrays to the GPU vertex and index buffers
        // TODO: keep an eye on whether any MonoGame update adds support for Span<byte> instead of byte[].
        // Then wouldn't need the intermediate arrays at all; could just set data in the loop above.
        _vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * ImDrawVertexStride);
        _indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
    }

    private void RenderCommandLists(ImDrawDataPtr drawData)
    {
        _graphicsDevice.SetVertexBuffer(_vertexBuffer);
        _graphicsDevice.Indices = _indexBuffer;

        int vtxOffset = 0;
        int idxOffset = 0;

        for (var cmdListIx = 0; cmdListIx < drawData.CmdListsCount; cmdListIx++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[cmdListIx];

            for (var cmdIx = 0; cmdIx < cmdList.CmdBuffer.Size; cmdIx++)
            {
                ImDrawCmdPtr cmd = cmdList.CmdBuffer[cmdIx];

                if (cmd.ElemCount == 0)
                {
                    continue;
                }

                if (!_texturesById.TryGetValue(cmd.TextureId, out var texture))
                {
                    throw new InvalidOperationException($"Could not find a texture with id '{cmd.TextureId}', please check your bindings");
                }

                _graphicsDevice.ScissorRectangle = new Rectangle(
                    (int)cmd.ClipRect.X,
                    (int)cmd.ClipRect.Y,
                    (int)(cmd.ClipRect.Z - cmd.ClipRect.X),
                    (int)(cmd.ClipRect.W - cmd.ClipRect.Y));

                _effect.Projection = Matrix.CreateOrthographicOffCenter(0f, _imGuiIO.DisplaySize.X, _imGuiIO.DisplaySize.Y, 0f, -1f, 1f);
                _effect.Texture = texture;

                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _graphicsDevice.DrawIndexedPrimitives(
                        primitiveType: PrimitiveType.TriangleList,
                        baseVertex: (int)cmd.VtxOffset + vtxOffset,
                        startIndex: (int)cmd.IdxOffset + idxOffset,
                        primitiveCount: (int)cmd.ElemCount / 3);
                }
            }

            vtxOffset += cmdList.VtxBuffer.Size;
            idxOffset += cmdList.IdxBuffer.Size;
        }
    }

    private void Dispose(bool isSafeToAccessReferences)
    {
        unsafe
        {
            if (_iniFilePathPtr != 0)
            {
                Marshal.FreeHGlobal(_iniFilePathPtr);
            }
        }

        ImGui.DestroyContext(_imGuiContext);

        if (isSafeToAccessReferences)
        {
            _game.Window.TextInput -= HandleWindowTextInput;

            foreach (var texture in _texturesById.Values)
            {
                texture?.Dispose();
            }
        }
    }
}
