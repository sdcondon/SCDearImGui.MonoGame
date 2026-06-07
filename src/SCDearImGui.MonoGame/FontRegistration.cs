using ImGuiNET;
using System.Runtime.InteropServices;

namespace SCDearImGui.MonoGame;

public class FontRegistration
{
    internal FontRegistration(string ttfFilePath, float defaultSizePixels)
    {
        TtfFilePath = ttfFilePath;
        DefaultSizePixels = defaultSizePixels;
    }

    internal FontRegistration(byte[] ttfData, float defaultSizePixels)
    {
        TtfData = ttfData;
        DefaultSizePixels = defaultSizePixels;
    }

    public string? TtfFilePath { get; }

    public byte[]? TtfData { get; }

    public float DefaultSizePixels { get; }

    public ImFontPtr FontPtr { get; internal set; }

    internal void AddToAtlas(ImFontAtlasPtr fontAtlasPtr, float scale)
    {
        if (TtfFilePath != null)
        {
            FontPtr = fontAtlasPtr.AddFontFromFileTTF(TtfFilePath, DefaultSizePixels * scale);
        }
        else if (TtfData != null)
        {
            // NB: note that we don't free the unmanaged memory that we allocate here.
            // It is used directly by ImGui rather than being the source of a copy - it is referred to as the "input data".
            // It is tidied by ImFontAtlas::Clear (among others) - which is invoked by ImGuiRenderer in ApplyStyleAndFonts.
            var data = Marshal.AllocHGlobal(TtfData.Length);
            Marshal.Copy(TtfData, 0, data, TtfData.Length);
            FontPtr = fontAtlasPtr.AddFontFromMemoryTTF(data, TtfData.Length, DefaultSizePixels * scale);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}
