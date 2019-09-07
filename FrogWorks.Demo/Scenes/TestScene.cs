using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class TestScene : Scene
    {
        private Player Player { get; set; }

        private World World { get; set; }

        private MonoFont Font { get; set; }

        public TestScene() 
            : base() { }

        protected override void Begin()
        {
            BackgroundColor = Color.Black;

            var layer = new Layer();
            var hudLayer = new Layer();
            Layers.Add(Extensions.AsEnumerable(layer, hudLayer));

            World = new World(10, 8, 32, 32);
            Player = new Player(World) { X = 64f, Y = 64f };
            Font = new MonoFont(304, 224) { X = 8, Y = 8 };

            layer.Entities.Add(Extensions.AsEnumerable<Entity>(World, Player));
            hudLayer.Entities.Add(Font);
        }

        protected override void AfterUpdate()
        {
            Font.Text = Player.ToString();
        }
    }
}
