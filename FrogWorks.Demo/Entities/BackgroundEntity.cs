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

        public BackgroundEntity()
            : base()
        {
            Background = new BackgroundImage(Texture, false);
            AddComponents(Background);
        }

        public override void Update(float deltaTime)
        {
            Background.Position += Vector2.One * .5f;
            Background.X = Background.X.Mod(Background.Texture.Width);
            Background.Y = Background.X.Mod(Background.Texture.Height);

            base.Update(deltaTime);
        }
    }
}
