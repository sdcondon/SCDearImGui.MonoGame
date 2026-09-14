using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using static ImGuiNET.ImGui;

namespace SCDearImGui.MonoGame.Demos.GuiElements.MiniApps;

// Demo of a window that includes a rendered 3D image.
class ModelViewerWindow(
    GraphicsDevice graphicsDevice,
    ContentManager contentManager,
    ImGuiRenderer imGuiRenderer,
    string modelAssetName,
    bool isOpen = false)
{
    public bool IsOpen = isOpen;

    private Model model;
    private Matrix modelWorld = Matrix.CreateRotationX(-1.5f);
    private Matrix modelProjection;
    private RenderTarget2D modelRenderTarget;
    private nint modelTextureId;

    public void LoadContent()
    {
        model = contentManager.Load<Model>(modelAssetName);
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects.Cast<BasicEffect>())
            {
                effect.TextureEnabled = false;
                effect.EnableDefaultLighting();
                effect.View = Matrix.CreateLookAt(new(0, 0, 4), Vector3.Zero, Vector3.UnitY);
            }
        }

        modelRenderTarget = new RenderTarget2D(
            graphicsDevice,
            graphicsDevice.PresentationParameters.BackBufferWidth,
            graphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        modelTextureId = imGuiRenderer.RegisterTexture(modelRenderTarget);
    }

    public void UnloadContent()
    {
        contentManager.UnloadAsset(modelAssetName);
        imGuiRenderer.UnregisterTexture(modelTextureId);
    }

    public void Update()
    {
        if (!IsOpen) return;

        SetNextWindowSize(new(430, 450), ImGuiCond.FirstUseEver);
        if (Begin("Example: Model Viewer", ref IsOpen))
        {
            System.Numerics.Vector2 imageSize = GetContentRegionAvail();
            if (imageSize.X > 0 && imageSize.Y > 0)
            {
                var cursorPos = GetCursorPos();

                InvisibleButton("model", imageSize, ImGuiButtonFlags.MouseButtonLeft);
                if (IsItemActive() && IsMouseDragging(ImGuiMouseButton.Left))
                {
                    var io = GetIO();
                    modelWorld *= Matrix.CreateRotationY(0.01f * io.MouseDelta.X) * Matrix.CreateRotationX(0.01f * io.MouseDelta.Y);
                }

                SetCursorPos(cursorPos);
                Image(modelTextureId, imageSize);

                modelProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), imageSize.X / imageSize.Y, 0.1f, 100.0f);
            }
        }

        End();
    }

    public void DrawModelToTexture()
    {
        if (!IsOpen) return;

        var priorRenderTargets = graphicsDevice.GetRenderTargets();

        graphicsDevice.SetRenderTarget(modelRenderTarget);
        graphicsDevice.Clear(Color.Transparent);

        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects.Cast<BasicEffect>())
            {
                effect.World = modelWorld;
                effect.Projection = modelProjection;
            }

            mesh.Draw();
        }

        graphicsDevice.SetRenderTargets(priorRenderTargets);
    }
}
