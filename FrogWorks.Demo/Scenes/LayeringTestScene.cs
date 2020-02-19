using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class LayeringTestScene : Scene
    {
        public LayeringTestScene()
            : base()
        {
        }

        protected override void Begin()
        {
            Add(new Layer() { RenderBeforeMerge = true });
            Add(new Layer());

            SetCurrentLayer(1);
            Add(new Apple() { X = 80f, Y = 96f });
            SetCurrentLayer(0);
            Add(new Apple() { X = 128f, Y = 112f });
            SetCurrentLayer(-1);
            Add(new Apple() { X = 176f, Y = 128f });

            ClearColor = Color.HotPink;
        }
    }
}
