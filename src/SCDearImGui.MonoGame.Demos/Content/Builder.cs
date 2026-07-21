using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace SCDearImGui.MonoGame.Demos.Content;

public class Builder : ContentBuilder
{
    public override IContentCollection GetContentCollection()
    {
        var content = new ContentCollection();

        content.Include("Models/suzanne.fbx", new FbxImporter(), new ModelProcessor()
        {
            ColorKeyColor = new(0, 0, 0, 0),
            ColorKeyEnabled = true,
            DefaultEffect = MaterialProcessorDefaultEffect.BasicEffect,
            GenerateMipmaps = true,
            GenerateTangentFrames = false,
            PremultiplyTextureAlpha = true,
            PremultiplyVertexColors = true,
            ResizeTexturesToPowerOfTwo = false,
            RotationX = 0,
            RotationY = 0,
            RotationZ = 0,
            Scale = 1,
            SwapWindingOrder = false,
            TextureFormat = TextureProcessorOutputFormat.Compressed,
        });

        content.IncludeCopy<WildcardRule>("Fonts/*.ttf");

        return content;
    }
}
