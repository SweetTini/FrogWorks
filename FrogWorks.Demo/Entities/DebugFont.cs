using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class DebugFont : Entity
    {
        private SpriteText _spriteText;

        private static BitmapFont BitmapFont { get; set; }

        public string Text
        {
            get { return _spriteText.Text; }
            set { _spriteText.Text = value; }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return _spriteText.HorizontalAlignment; }
            set { _spriteText.HorizontalAlignment = value; }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return _spriteText.VerticalAlignment; }
            set { _spriteText.VerticalAlignment = value; }
        }

        public Color Color
        {
            get { return _spriteText.Color; }
            set { _spriteText.Color = value; }
        }

        public DebugFont(int width, int height)
            : base()
        {
            if (BitmapFont == null)
            {
                var texture = Texture.Load(@"Textures/DebugFont.png");
                var charSet = " !\"#$%&\'()*+,-./0123456789:;<=>?" 
                            + "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`"
                            + "abcdefghijklmnopqrstuvwxyz{|}~";
                BitmapFont = new BitmapFont(texture, 8, 8, charSet);
            }

            _spriteText = new SpriteText(BitmapFont, string.Empty, width, height);
            Components.Add(_spriteText);
        }
    }
}
