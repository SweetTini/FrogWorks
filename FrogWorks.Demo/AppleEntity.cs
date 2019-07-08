using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class AppleEntity : Entity
    {
        public Image Image { get; private set; }

        public SpriteText Text { get; private set; }

        public Shaker Shaker { get; private set; }

        public AppleEntity()
            : base()
        {
            Image = new Image(Texture.Load("Images\\Apple.png"), false);
            Image.CenterOrigin();

            var charSet = " !\"\'*+,-./0123456789:;ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var font = new BitmapFont(Texture.Load("Images\\MonoFont.png"), 10, 16, charSet);
            Text = new SpriteText(font, string.Empty, 36, 64);
            Text.Y = 6;
            Text.HorizontalAlignment = HorizontalAlignment.Center;
            Text.VerticalAlignment = VerticalAlignment.Center;
            Text.CenterOrigin();

            Shaker = Shaker.CreateAndApply(this, 30f, ShakeImage);

            AddComponents(Image, Text);
        }

        protected void ShakeImage(Vector2 offset)
        {
            Image.Position += offset;
            Text.Position += offset;
            var time = Shaker.TimeLeft.ToString("0.0");
            Text.Text = $"{time}";
        }
    }
}
