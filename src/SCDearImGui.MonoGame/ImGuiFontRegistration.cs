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
    private readonly FontSpec baseSpec;

    private readonly List<FontSpec> mergeSpecs = [];

    internal ImGuiFontRegistration(byte[] ttfData, float defaultSizePixels, IEnumerable<(char start, char end)>? extraGlyphRanges = null)
    {
        this.baseSpec = new(ttfData, defaultSizePixels, true, extraGlyphRanges?.ToArray());
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

    /// <summary>
    /// Merges specific glyphs from another font into this one.
    /// </summary>
    /// <param name="ttfData">The ttf data of the font to merge.</param>
    /// <param name="defaultSizePixels">The default size (i.e. when the GUI scale is set to 1) to use for the merged glyphs, in pixels.</param>
    /// <param name="glyphRanges">The ranges of glyphs to merge in from the font.</param>
    public void Merge(byte[] ttfData, float defaultSizePixels, IEnumerable<(char start, char end)>? glyphRanges = null)
    {
        mergeSpecs.Add(new(ttfData, defaultSizePixels, false, glyphRanges?.ToArray())
        {
            IsMerge = true,
        });
    }

    internal void AddToAtlas(ImFontAtlasPtr fontAtlasPtr, float scale)
    {
        // NB: note that we don't free the unmanaged memory that we allocate here.
        // It is used directly by ImGui rather than being the source of a copy - it is referred to as the "input data".
        // It is tidied by ImFontAtlas::Clear (among others) - which is invoked by ImGuiRenderer in ApplyStyleAndFonts.
        var data = Marshal.AllocHGlobal(baseSpec.Data.Length);
        Marshal.Copy(baseSpec.Data, 0, data, baseSpec.Data.Length);
        CurrentFontPtr = fontAtlasPtr.AddFontFromMemoryTTF(data, baseSpec.Data.Length, baseSpec.Size * scale, baseSpec.ImFontConfigPtr);

        foreach (var mergeSpec in mergeSpecs)
        {
            data = Marshal.AllocHGlobal(mergeSpec.Data.Length);
            Marshal.Copy(mergeSpec.Data, 0, data, mergeSpec.Data.Length);
            fontAtlasPtr.AddFontFromMemoryTTF(data, mergeSpec.Data.Length, mergeSpec.Size * scale, mergeSpec.ImFontConfigPtr);
        }
    }

    private class FontSpec
    {
        private readonly ImFontConfigPtr imFontConfigPtr;

        public FontSpec(byte[] data, float size, bool includeDefaultGlyphRange, (char start, char end)[]? glyphRanges = null)
        {
            Data = data;
            Size = size;

            unsafe
            {
                void* glyphRangesPtr = null;
                ImFontGlyphRangesBuilderPtr rangesBuilderPtr = new(null);

                try
                {
                    imFontConfigPtr = new(ImGuiNative.ImFontConfig_ImFontConfig());

                    rangesBuilderPtr = new(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());

                    if (includeDefaultGlyphRange)
                    {
                        rangesBuilderPtr.AddRanges(ImGui.GetIO().Fonts.GetGlyphRangesDefault());
                    }

                    if (glyphRanges != null)
                    {
                        var glyphRangesElementCount = glyphRanges.Length * 2 + 1;
                        glyphRangesPtr = NativeMemory.Alloc((nuint)glyphRangesElementCount, sizeof(char));
                        var glyphRangesSpan = new Span<char>(glyphRangesPtr, glyphRangesElementCount);
                        for (int i = 0; i < glyphRanges.Length; i++)
                        {
                            glyphRangesSpan[2 * i] = glyphRanges[i].start;
                            glyphRangesSpan[2 * i + 1] = glyphRanges[i].end;
                        }
                        glyphRangesSpan[^1] = '\0';
                        rangesBuilderPtr.AddRanges((nint)glyphRangesPtr);
                    }

                    rangesBuilderPtr.BuildRanges(out var ranges);
                    imFontConfigPtr.GlyphRanges = ranges.Data;
                }
                catch
                {
                    imFontConfigPtr.Destroy();
                    throw;
                }
                finally
                {
                    if (glyphRangesPtr != null)
                    {
                        NativeMemory.Free(glyphRangesPtr);
                    }

                    if (rangesBuilderPtr.NativePtr != null)
                    {
                        rangesBuilderPtr.Destroy();
                    }
                }
            }
        }

        // NB: don't bother making disposable just yet. prob should at some point.
        ~FontSpec()
        {
            ImFontConfigPtr.Destroy();
        }

        public byte[] Data { get; }

        public float Size { get; }

        public bool IsMerge
        {
            get => imFontConfigPtr.MergeMode;
            set => imFontConfigPtr.MergeMode = value;
        }

        public ImFontConfigPtr ImFontConfigPtr => imFontConfigPtr;
    }
}
