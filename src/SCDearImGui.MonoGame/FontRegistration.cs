using ImGuiNET;
using System.Runtime.InteropServices;

namespace SCDearImGui.MonoGame;

/// <summary>
/// <para>
/// A representation of a font registered with <see cref="ImGuiRenderer.RegisterFont"/>.
/// </para>
/// <para>
/// The purpose of this class is to provide indirected access to the <see cref="ImFontPtr"/>, so that
/// consumers don't need to concern themselves with it changing when <see cref="ImGuiRenderer.ApplyStyleAndFonts(float)"/>
/// is called. As such, *do not* copy out <see cref="CurrentFontPtr"/> - access this property directly whenever you need it.
/// </para>
/// </summary>
public class FontRegistration
{
    private readonly string? ttfFilePath;
    private readonly byte[]? ttfData;
    private readonly float defaultSizePixels;

    internal FontRegistration(string ttfFilePath, float defaultSizePixels)
    {
        this.ttfFilePath = ttfFilePath;
        this.defaultSizePixels = defaultSizePixels;
    }

    internal FontRegistration(byte[] ttfData, float defaultSizePixels)
    {
        this.ttfData = ttfData;
        this.defaultSizePixels = defaultSizePixels;
    }

    /// <summary>
    /// The current font pointer for this registration. Note that <strong>this will change</strong>
    /// whenever <see cref="ImGuiRenderer.ApplyStyleAndFonts(float)"/> is called. As such, do not
    /// copy it out anywhere else - use it directly whenever you need it.
    /// </summary>
    public ImFontPtr CurrentFontPtr { get; private set; }

    internal void AddToAtlas(ImFontAtlasPtr fontAtlasPtr, float scale)
    {
        if (ttfFilePath != null)
        {
            CurrentFontPtr = fontAtlasPtr.AddFontFromFileTTF(ttfFilePath, defaultSizePixels * scale);
        }
        else if (ttfData != null)
        {
            // NB: note that we don't free the unmanaged memory that we allocate here.
            // It is used directly by ImGui rather than being the source of a copy - it is referred to as the "input data".
            // It is tidied by ImFontAtlas::Clear (among others) - which is invoked by ImGuiRenderer in ApplyStyleAndFonts.
            var data = Marshal.AllocHGlobal(ttfData.Length);
            Marshal.Copy(ttfData, 0, data, ttfData.Length);
            CurrentFontPtr = fontAtlasPtr.AddFontFromMemoryTTF(data, ttfData.Length, defaultSizePixels * scale);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}
