using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class TestScene : Scene
    {
        public TestScene()
            : base() { }

        protected override void Begin()
        {
            BackgroundColor = Color.Gray;

            var layer = new Layer();
            Layers.Add(layer);

            var appleA = new Apple() { X = 135f, Y = 115f };
            var appleB = new Apple() { X = 160f, Y = 144f };
            var appleC = new Apple() { X = 135f, Y = 160f };
            layer.Entities.Add(appleA);
            layer.Entities.Add(appleB);
            layer.Entities.Add(appleC);

            layer.Entities.MoveToTop(appleA);
            layer.Entities.MoveAhead(appleB, appleC);
        }
    }
}
