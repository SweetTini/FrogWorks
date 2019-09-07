using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class MonoFont : Entity
    {
        private SpriteText _spriteText;

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

        public MonoFont(int width, int height)
            : base()
        {
            _spriteText = new SpriteText(DefaultFont.Font, string.Empty, width, height);
            Components.Add(_spriteText);
        }
    }
}
