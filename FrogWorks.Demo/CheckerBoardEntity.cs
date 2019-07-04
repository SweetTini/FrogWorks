using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class CheckerBoardEntity : Entity
    {
        public BackgroundImage Image { get; private set; }

        public CheckerBoardEntity()
            : base()
        {
            Image = new BackgroundImage(Texture.Load("Images\\Checker.png"), false);
            AddComponent(Image);
        }
    }
}
