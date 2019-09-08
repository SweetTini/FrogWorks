using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class TestScene : Scene
    {
        private Player Player { get; set; }

        private World World { get; set; }

        private MonoFont Font { get; set; }

        private MonoFont FpsFont { get; set; }

        public TestScene() 
            : base() { }

        protected override void Begin()
        {
            BackgroundColor = Color.CornflowerBlue;

            var layer = new Layer();
            var hudLayer = new Layer();
            Layers.Add(Extensions.AsEnumerable(layer, hudLayer));

            World = new World(20, 15, 32, 32);
            Player = new Player(World) { X = 64f, Y = 64f };
            Font = new MonoFont(304, 224) { X = 8, Y = 8 };
            FpsFont = new MonoFont(304, 224) { X = 8, Y = 8, HorizontalAlignment = HorizontalAlignment.Right };

            layer.Entities.Add(Extensions.AsEnumerable<Entity>(World, Player));
            hudLayer.Entities.Add(Extensions.AsEnumerable<Entity>(Font, FpsFont));

            layer.Camera.SetZone(World.Collider.Size.ToPoint());
        }

        protected override void AfterUpdate()
        {
            Font.Text = Player.ToString();
            FpsFont.Text = $"{Runner.Application.FramesPerSecond}fps";
        }
    }
}
