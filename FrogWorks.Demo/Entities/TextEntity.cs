using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class TextEntity : Entity
    {
        protected SpriteText SpriteText { get; private set; }

        public string Text
        {
            get { return SpriteText.Text; }
            set { SpriteText.Text = value; }
        }

        public Color Color
        {
            get { return SpriteText.Color; }
            set { SpriteText.Color = value; }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return SpriteText.HorizontalAlignment; }
            set { SpriteText.HorizontalAlignment = value; }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return SpriteText.VerticalAlignment; }
            set { SpriteText.VerticalAlignment = value; }
        }

        public TextEntity()
            : base()
        {
            SpriteText = new SpriteText(DefaultFont.Font, string.Empty, Engine.FrameWidth);
            AddComponents(SpriteText);
        }
    }
}
