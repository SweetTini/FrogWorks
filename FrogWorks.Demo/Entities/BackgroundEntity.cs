using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class BackgroundEntity : Entity
    {
        protected static Texture Texture { get; } = Texture.Load(@"Images\Checker.png");

        protected BackgroundImage Background { get; private set; }

        public Color Color
        {
            get { return Background.Color; }
            set { Background.Color = value; }
        }

        public Vector2 Coefficient { get; set; } = Vector2.One;

        public bool Autoscroll { get; set; }

        public BackgroundEntity()
            : base()
        {
            Background = new BackgroundImage(Texture, false);
            AddComponents(Background);
        }

        public override void Update(float deltaTime)
        {
            if (Autoscroll)
            {
                Background.Position += Coefficient;
                Background.X = Background.X.Mod(Background.Texture.Width);
                Background.Y = Background.X.Mod(Background.Texture.Height);
            }

            base.Update(deltaTime);
        }
    }
}
