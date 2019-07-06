using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class SpriteText : GraphicsComponent
    {
        private string _text;
        private int _width, _height;
        private HorizontalAlignment _horizAlign;
        private VerticalAlignment _vertAlign;
        private bool _wordWrap;

        protected List<SpriteTextCharacter> Characters { get; private set; }

        protected BitmapFont Font { get; private set; }

        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        public Rectangle AbsoluteBounds => Bounds.Transform(DrawPosition, Origin, Scale, Angle);

        protected bool IsDirty { get; private set; }

        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
                IsDirty = true;
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                value = Math.Abs(value);

                if (value == _width) return;
                _width = value;
                IsDirty = true;
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                value = Math.Abs(value);

                if (value == _height) return;
                _height = value;
                IsDirty = true;
            }
        }

        public int Spacing
        {
            get { return Font.Spacing; }
            set
            {
                if (value == Font.Spacing) return;
                Font.Spacing = value;
                IsDirty = true;
            }
        }

        public int LineHeight
        {
            get { return Font.LineHeight; }
            set
            {
                if (value == Font.LineHeight) return;
                Font.LineHeight = value;
                IsDirty = true;
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizAlign; }
            set
            {
                if (value == _horizAlign) return;
                _horizAlign = value;
                IsDirty = true;
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return _vertAlign; }
            set
            {
                if (value == _vertAlign) return;
                _vertAlign = value;
                IsDirty = true;
            }
        }

        public bool WordWrap
        {
            get { return _wordWrap; }
            set
            {
                if (value == _wordWrap) return;
                _wordWrap = value;
                IsDirty = true;
            }
        }

        public SpriteText(BitmapFont font, string text, int width = 0, int height = 0)
            : base(true)
        {
            Characters = new List<SpriteTextCharacter>();
            Font = font;
            Text = text;
            Width = width;
            Height = height;
        }

        public override void Draw(RendererBatch batch)
        {
            if (IsDirty) Refresh();

            for (int i = 0; i < Characters.Count; i++)
            {
                var origin = Origin - Characters[i].Offset;
                Characters[i].Source.Texture.Draw(batch, DrawPosition, origin, Scale, Angle, Color * MathHelper.Clamp(Opacity, 0f, 1f), SpriteEffects);
            }
        }

        public void SetOrigin(Origin origin)
        {
            switch (origin)
            {
                case FrogWorks.Origin.TopLeft:
                    Origin = new Vector2(Bounds.Left, Bounds.Top);
                    break;
                case FrogWorks.Origin.Top:
                    Origin = new Vector2(Bounds.Center.X, Bounds.Top);
                    break;
                case FrogWorks.Origin.TopRight:
                    Origin = new Vector2(Bounds.Right, Bounds.Top);
                    break;
                case FrogWorks.Origin.Left:
                    Origin = new Vector2(Bounds.Left, Bounds.Center.Y);
                    break;
                case FrogWorks.Origin.Center:
                    Origin = Bounds.Center.ToVector2();
                    break;
                case FrogWorks.Origin.Right:
                    Origin = new Vector2(Bounds.Right, Bounds.Center.Y);
                    break;
                case FrogWorks.Origin.BottomLeft:
                    Origin = new Vector2(Bounds.Left, Bounds.Bottom);
                    break;
                case FrogWorks.Origin.Bottom:
                    Origin = new Vector2(Bounds.Center.X, Bounds.Bottom);
                    break;
                case FrogWorks.Origin.BottomRight:
                    Origin = new Vector2(Bounds.Right, Bounds.Bottom);
                    break;
            }
        }

        public void CenterOrigin()
        {
            Origin = Bounds.Center.ToVector2();
        }

        protected void Refresh()
        {
            Characters.Clear();

            if (_wordWrap && (_width == 0 || _height == 0))
                _wordWrap = false;

            var text = _wordWrap ? Font.WordWrap(Text, Width) : Text;
            var offset = Vector2.UnitY * Font.MeasureVerticalOffset(_vertAlign, text, _height);
            var lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                offset.X = Font.MeasureHorizontalOffset(_horizAlign, line, _width);

                for (int j = 0; j < line.Length; j++)
                {
                    var ascii = line[j];
                    var character = Font[ascii];
                    if (character == null) continue;

                    var kerning = 0;
                    if (j < line.Length - 1)
                        character.Kernings.TryGetValue(line[j + 1], out kerning);
                    var charOffset = offset + character.Offset.ToVector2();

                    Characters.Add(new SpriteTextCharacter(character, charOffset));
                    offset.X += kerning + character.Spacing + Font.Spacing;
                }

                offset.Y += Font.DefaultLineHeight + Font.LineHeight;
            }

            IsDirty = false;
        }

        protected void MarkAsDirty()
        {
            IsDirty = true;
        }
    }

    public struct SpriteTextCharacter
    {
        public BitmapCharacter Source { get; internal set; }

        public Vector2 Offset { get; internal set; }

        internal SpriteTextCharacter(BitmapCharacter source, Vector2 offset)
        {
            Source = source;
            Offset = offset;
        }
    }
}
