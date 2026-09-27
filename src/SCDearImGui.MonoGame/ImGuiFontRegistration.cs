using ImGuiNET;
using System.Runtime.InteropServices;

namespace SCDearImGui.MonoGame;

/// <summary>
/// <para>
/// A representation of a font registered with <see cref="ImGuiRenderer.RegisterFont"/>.
/// </para>
/// <para>
/// The purpose of this class is to provide indirected access to the <see cref="ImFontPtr"/>, so that
/// consumers don't need to concern themselves with keeping it up to date (it will change whenever 
/// <see cref="ImGuiRenderer.ApplyStyleAndFonts(float)"/> is called).
/// </para>
/// </summary>
public class ImGuiFontRegistration
{
    private readonly string? ttfFilePath;
    private readonly byte[]? ttfData;
    private readonly float defaultSizePixels;
    private readonly (char start, char end)[]? extraGlyphRanges;

    internal ImGuiFontRegistration(string ttfFilePath, float defaultSizePixels, IEnumerable<(char start, char end)>? extraGlyphRanges = null)
    {
        this.ttfFilePath = ttfFilePath;
        this.defaultSizePixels = defaultSizePixels;
        this.extraGlyphRanges = extraGlyphRanges?.ToArray();
    }

    internal ImGuiFontRegistration(byte[] ttfData, float defaultSizePixels, IEnumerable<(char start, char end)>? extraGlyphRanges = null)
    {
        this.ttfData = ttfData;
        this.defaultSizePixels = defaultSizePixels;
        this.extraGlyphRanges = extraGlyphRanges?.ToArray();
    }

    /// <summary>
    /// <para>
    /// The current font pointer for this registration. 
    /// </para>
    /// <para>
    /// Note that <strong>this will change</strong> whenever <see cref="ImGuiRenderer.ApplyStyleAndFonts(float)"/>
    /// is called. As such, do not copy it out anywhere else - use it directly whenever you need it.
    /// </para>
    /// </summary>
    public ImFontPtr CurrentFontPtr { get; private set; }

    internal void AddToAtlas(ImFontAtlasPtr fontAtlasPtr, float scale)
    {
        unsafe
        {
            void* extraRangesPtr = null;

            try
            {
                ImFontConfigPtr configPtr = new(ImGuiNative.ImFontConfig_ImFontConfig());

                if (extraGlyphRanges != null)
                {
                    ImFontGlyphRangesBuilderPtr b = new(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
                    b.AddRanges(ImGui.GetIO().Fonts.GetGlyphRangesDefault());

                    var extraRangesElementCount = extraGlyphRanges.Length * 2 + 1;
                    extraRangesPtr = NativeMemory.Alloc((nuint)extraRangesElementCount, sizeof(char));
                    var extraRangesSpan = new Span<char>(extraRangesPtr, extraRangesElementCount);
                    for (int i = 0; i < extraGlyphRanges.Length; i++)
                    {
                        extraRangesSpan[2 * i] = extraGlyphRanges[i].start;
                        extraRangesSpan[2 * i + 1] = extraGlyphRanges[i].end;
                    }
                    extraRangesSpan[^1] = '\0';
                    b.AddRanges((nint)extraRangesPtr);

                    // TODO: b.AddRanges(..) - looks like imgui expects 0-terminated array of pairs of low-high.
                    // ALSO TODO: think i do actually need to do some tidy up. make disposable. free ranges data on dispose.
                    b.BuildRanges(out var ranges);

                    configPtr.GlyphRanges = ranges.Data;
                }

                if (ttfFilePath != null)
                {
                    CurrentFontPtr = fontAtlasPtr.AddFontFromFileTTF(ttfFilePath, defaultSizePixels * scale, configPtr);
                }
                else if (ttfData != null)
                {
                    // NB: note that we don't free the unmanaged memory that we allocate here.
                    // It is used directly by ImGui rather than being the source of a copy - it is referred to as the "input data".
                    // It is tidied by ImFontAtlas::Clear (among others) - which is invoked by ImGuiRenderer in ApplyStyleAndFonts.
                    var data = Marshal.AllocHGlobal(ttfData.Length);
                    Marshal.Copy(ttfData, 0, data, ttfData.Length);
                    CurrentFontPtr = fontAtlasPtr.AddFontFromMemoryTTF(data, ttfData.Length, defaultSizePixels * scale, configPtr);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            finally
            {
                if (extraRangesPtr != null)
                {
                    NativeMemory.Free(extraRangesPtr);
                }
            }
        }
    }
}
