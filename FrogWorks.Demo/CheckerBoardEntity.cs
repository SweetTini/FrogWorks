using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class CheckerBoardEntity : Entity
    {
        public TiledImage Image { get; private set; }

        public CheckerBoardEntity()
            : base()
        {
            Image = new TiledImage(Texture.Load("Images\\Checker.png"), 100, 100, false);
            AddComponent(Image);
        }
    }
}
