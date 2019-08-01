using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class Text : Entity
    {
        protected SpriteText SpriteText { get; private set; }

        public string TextToDisplay
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

        public Text()
            : base()
        {
            SpriteText = new SpriteText(DefaultFont.Font, string.Empty, Engine.Display.Width);
            AddComponents(SpriteText);
        }
    }
}
