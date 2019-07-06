namespace FrogWorks.Demo
{
    public class AppleEntity : Entity
    {
        public Image Image { get; private set; }

        public SpriteText Text { get; private set; }

        public AppleEntity()
            : base()
        {
            Image = new Image(Texture.Load("Images\\Apple.png"), false);
            Image.CenterOrigin();

            var charSet = " !\"\'*+,-.0123456789:;?ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var font = new BitmapFont(Texture.Load("Images\\MonoFont.png"), 12, 16, charSet);
            Text = new SpriteText(font, "30", 36, 64);
            Text.HorizontalAlignment = HorizontalAlignment.Center;
            Text.VerticalAlignment = VerticalAlignment.Center;
            Text.CenterOrigin();

            AddComponents(Image, Text);
        }
    }
}
