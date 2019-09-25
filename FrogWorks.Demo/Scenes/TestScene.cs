using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class TestScene : Scene
    {
        private MonoFont FpsFont { get; set; }

        public TestScene() 
            : base() { }

        protected override void Begin()
        {
            BackgroundColor = Color.CornflowerBlue;

            var mainLayer = new BasicLayer();
            var hudLayer = new BasicLayer();
            Layers.Add(mainLayer, hudLayer);

            var container = TiledLoader.Load("Maps/TestMap.tmx");
            var renderer = new TileMapRenderer();
            renderer.Add(container.TileLayers["background"]);
            var world = new World(container);
            var player = new Player(world) { X = 64f, Y = 64f };
            container.ProcessDataLayer("blocks", world.Configure);
            mainLayer.Entities.Add(renderer, world, player);
            mainLayer.Camera.SetZone(world.Size.ToPoint());

            FpsFont = new MonoFont(304, 224) { X = 8, Y = 8, HorizontalAlignment = HorizontalAlignment.Right };
            hudLayer.Entities.Add(FpsFont);
        }

        protected override void AfterUpdate()
        {
            FpsFont.Text = $"{Runner.Application.FramesPerSecond}fps";
        }
    }
}
