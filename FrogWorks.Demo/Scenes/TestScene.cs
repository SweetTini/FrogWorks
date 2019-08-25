using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class TestScene : Scene
    {
        private Apple Apple { get; set; }

        public TestScene()
            : base() { }

        protected override void Begin()
        {
            BackgroundColor = Color.Gray;

            var layerA = new Layer();
            var layerB = new Layer();
            Layers.Add(layerA);
            Layers.Add(layerB);

            var appleA = new Apple() { X = 150f, Y = 100f };
            var appleB = new Apple() { X = 170f, Y = 120f };
            var appleC = new Apple() { X = 150f, Y = 140f };
            layerA.Entities.Add(appleA);
            layerA.Entities.Add(appleB);
            layerB.Entities.Add(appleC);

            layerA.Entities.MoveToTop(appleA);
            layerA.Entities.SwitchToLayer(appleB, layerB);
            layerB.Entities.MoveToTop(appleB);

            Apple = appleA;
        }

        protected override void BeforeUpdate()
        {
            Apple.X += Input.Keyboard.GetAxis(Keys.RightArrow, Keys.LeftArrow) * 2f;
            Apple.Y += Input.Keyboard.GetAxis(Keys.DownArrow, Keys.UpArrow) * 2f;
        }
    }
}
