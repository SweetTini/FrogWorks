using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public abstract class DefaultScene : Scene
    {
        protected BasicLayer DefaultLayer { get; private set; }

        protected DefaultScene()
            : base()
        {
            BackgroundColor = Color.Black;
            DefaultLayer = new BasicLayer();
            Layers.Add(DefaultLayer);
        }
    }
}
