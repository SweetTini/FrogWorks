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
            //var mapTest = Tiled.Load("Maps\\TestArea1.tmx");
            BackgroundColor = Color.CornflowerBlue;

            var backgroundLayer = new BasicLayer() { ScrollRate = Vector2.One * .4f };
            var mainLayer = new BasicLayer();
            var hudLayer = new BasicLayer() { ScrollRate = Vector2.Zero };
            Layers.Add(backgroundLayer, mainLayer, hudLayer);

            var checker = new Checker();
            var world = new World(30, 10, 32, 32);
            var player = new Player(world) { X = 64f, Y = 64f };
            backgroundLayer.Entities.Add(checker);
            mainLayer.Entities.Add(world, player);
            Camera.SetZone(world.Size.ToPoint());

            FpsFont = new MonoFont(304, 224) { X = 8, Y = 8, HorizontalAlignment = HorizontalAlignment.Right };
            hudLayer.Entities.Add(FpsFont);
        }

        protected override void AfterUpdate()
        {
            FpsFont.Text = $"{Runner.Application.FramesPerSecond}fps";
        }
    }
}
