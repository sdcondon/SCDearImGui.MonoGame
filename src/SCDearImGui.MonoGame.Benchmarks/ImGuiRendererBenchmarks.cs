using BenchmarkDotNet.Attributes;
using Microsoft.Xna.Framework;
using SCDearImGui.MonoGame.Benchmarks.Alts;
using System.Diagnostics.CodeAnalysis;

namespace SCDearImGui.MonoGame.Benchmarks;

// weirdness: first one (only) to run seems a bit slower? So put imgui.net one first cos thats way slower anyways - but of course fair test means need to switch em up to be sure..
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Benchmark methods cannot be static")]
public class ImGuiRendererBenchmarks
{
    static readonly FromImGuiNetGame fromImGuiNetGame = new();
    static readonly WithIOStateIndirectionGame withIOStateIndirectionGame = new();
    static readonly ProductionGame productionGame = new();

    [Benchmark]
    public void FromImGuiNET() => fromImGuiNetGame.RunOneFrame();

    [Benchmark]
    public void WithIOStateIndirection() => withIOStateIndirectionGame.RunOneFrame();

    [Benchmark(Baseline = true)]
    public void Production() => productionGame.RunOneFrame();

    private class ProductionGame : Game
    {
        readonly ImGuiRenderer renderer;

        public ProductionGame()
        {
            new GraphicsDeviceManager(this).ApplyChanges();
            renderer = new(this);
            renderer.ApplyStyleAndFonts();
            IsFixedTimeStep = false;
        }

        protected override void Update(GameTime gameTime)
        {
            renderer.BeginUpdate(gameTime);
            renderer.EndUpdate();
            SuppressDraw();
        }
    }

    private class WithIOStateIndirectionGame : Game
    {
        readonly ImGuiRenderer_WithIOStateIndirection renderer;

        public WithIOStateIndirectionGame()
        {
            new GraphicsDeviceManager(this).ApplyChanges();
            renderer = new(this);
            renderer.BuildFontAtlas();
            IsFixedTimeStep = false;
        }

        protected override void Update(GameTime gameTime)
        {
            renderer.BeginUpdate(gameTime);
            renderer.EndUpdate();
            SuppressDraw();
        }
    }

    private class FromImGuiNetGame : Game
    {
        readonly ImGuiRenderer_FromImGuiNET renderer;

        public FromImGuiNetGame()
        {
            new GraphicsDeviceManager(this).ApplyChanges();
            renderer = new(this);
            renderer.RebuildFontAtlas();
            IsFixedTimeStep = false;
        }

        protected override void Update(GameTime gameTime)
        {
            renderer.BeforeLayout(gameTime);
            renderer.AfterLayout();
            SuppressDraw();
        }
    }
}
