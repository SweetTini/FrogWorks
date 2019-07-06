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
            Text = new SpriteText(font, string.Empty, 36, 64);
            Text.Y = 6;
            Text.HorizontalAlignment = HorizontalAlignment.Center;
            Text.VerticalAlignment = VerticalAlignment.Center;
            Text.CenterOrigin();

            AddComponents(Image, Text);
        }

        public override void Update(float deltaTime)
        {
            Text.Text = $"{Depth}";
        }
    }
}
