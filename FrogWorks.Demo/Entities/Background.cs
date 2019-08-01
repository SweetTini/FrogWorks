using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class Background : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\Checker.png");

        protected BackgroundImage BackgroundImage { get; private set; }

        public Color Color
        {
            get { return BackgroundImage.Color; }
            set { BackgroundImage.Color = value; }
        }

        public Vector2 Coefficient { get; set; } = Vector2.One;

        public bool AutoScroll { get; set; }

        public Background()
            : base()
        {
            BackgroundImage = new BackgroundImage(Texture, false);
            AddComponents(BackgroundImage);
        }

        public override void Update(float deltaTime)
        {
            if (AutoScroll)
            {
                BackgroundImage.Position += Coefficient;
                BackgroundImage.X = BackgroundImage.X.Mod(BackgroundImage.Texture.Width);
                BackgroundImage.Y = BackgroundImage.X.Mod(BackgroundImage.Texture.Height);
            }

            base.Update(deltaTime);
        }
    }
}
