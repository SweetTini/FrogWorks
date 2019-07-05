using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FrogWorks
{
    public class BitmapFont
    {
        private StringBuilder _builder;
        private int _charSpacing;

        public BitmapCharacter this[int ascii]
        {
            get { return Characters.ContainsKey(ascii) ? Characters[ascii] : null; }
        }

        protected Dictionary<int, BitmapCharacter> Characters { get; private set; }

        public int Spacing { get; set; }

        public int DefaultLineHeight { get; internal set; }

        public int LineHeight { get; set; }

        public bool IsMonospace { get; internal set; }

        private BitmapFont()
        {
            _builder = new StringBuilder();
            Characters = new Dictionary<int, BitmapCharacter>();
        }

        public BitmapFont(Texture texture, int charWidth, int charHeight, string charSet)
        {
            var textures = Texture.Split(texture, charWidth, charHeight);
            var charCount = Math.Min(charSet.Length, textures.Length);

            for (int i = 0; i < charSet.Length; i++)
                Characters.Add(charSet[i], new BitmapCharacter(textures[i], charSet[i], spacing: charWidth));

            _charSpacing = charWidth;
            DefaultLineHeight = charHeight;
            IsMonospace = true;
        }

        public void Draw(RendererBatch batch, string text, Rectangle bounds, HorizontalAlignment alignX = HorizontalAlignment.Left, VerticalAlignment alignY = VerticalAlignment.Top, bool wordWrap = false, Vector2? origin = null, Vector2? scale = null, float angle = 0f, Color? color = null, SpriteEffects effects = SpriteEffects.None)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (wordWrap) text = WordWrap(text, bounds.Width);
            var offset = Vector2.UnitY * MeasureVerticalOffset(alignY, text, bounds.Height);
            var lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                offset.X = MeasureHorizontalOffset(alignX, line, bounds.Width);

                for (int j = 0; j < line.Length; j++)
                {
                    BitmapCharacter character;

                    if (Characters.TryGetValue(line[j], out character))
                    {
                        var kerning = 0;
                        if (j < line.Length - 1)
                            character.Kernings.TryGetValue(line[j + 1], out kerning);

                        var charPosition = bounds.Location.ToVector2();
                        var charOrigin = (origin ?? Vector2.Zero) - (offset + character.Offset.ToVector2());

                        character.Texture.Draw(batch, charPosition, charOrigin, scale ?? Vector2.One, angle, color ?? Color.White, effects);
                        offset.X += kerning + character.Spacing + Spacing;
                    }
                }

                offset.Y += DefaultLineHeight + LineHeight;
            }
        }

        public void Draw(RendererBatch batch, string text, int x, int y, int width, int height, HorizontalAlignment alignX = HorizontalAlignment.Left, VerticalAlignment alignY = VerticalAlignment.Top, bool wordWrap = false, Vector2? origin = null, Vector2? scale = null, float angle = 0f, Color? color = null, SpriteEffects effects = SpriteEffects.None)
        {
            Draw(batch, text, new Rectangle(x, y, width, height), alignX, alignY, wordWrap, origin, scale, angle, color, effects);
        }

        public void Configure(int ascii, int offsetX, int offsetY, int spacing)
        {
            BitmapCharacter character;

            if (Characters.TryGetValue(ascii, out character))
            {
                character.Offset = new Point(offsetX, offsetY);
                character.Spacing = spacing;

                if (character.Spacing != _charSpacing)
                    IsMonospace = false;
            }
        }

        public Point Measure(char ascii)
        {
            BitmapCharacter character;

            if (Characters.TryGetValue(ascii, out character))
                return new Point(character.Spacing + Spacing, DefaultLineHeight + LineHeight);

            return Point.Zero;
        }

        public Point Measure(string text)
        {
            if (string.IsNullOrEmpty(text)) return Point.Zero;

            var size = new Point(0, DefaultLineHeight + LineHeight);
            var width = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    size.X = Math.Max(size.X, width);
                    size.Y += DefaultLineHeight + LineHeight;
                    width = 0;
                }
                else
                {
                    BitmapCharacter character;

                    if (Characters.TryGetValue(text[i], out character))
                    {
                        var kerning = 0;
                        if (i < text.Length - 1)
                            character.Kernings.TryGetValue(text[i + 1], out kerning);
                        width += kerning + character.Spacing + Spacing;
                    }
                }
            }

            size.X = Math.Max(size.X, width);
            return size;
        }

        public int MeasureWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            var width = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n') break;

                BitmapCharacter character;

                if (Characters.TryGetValue(text[i], out character))
                {
                    var kerning = 0;
                    if (i < text.Length - 1)
                        character.Kernings.TryGetValue(text[i + 1], out kerning);
                    width += kerning + character.Spacing + Spacing;
                }
            }

            return width;
        }

        public int MeasureHeight(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            var lines = 1;

            if (text.IndexOf('\n') > -1)
                for (int i = 0; i < text.Length; i++)
                    if (text[i] == '\n')
                        lines++;

            return lines * LineHeight;
        }

        public int MeasureHorizontalOffset(HorizontalAlignment alignment, string line, int width)
        {
            var lineWidth = MeasureWidth(line);
            var offset = 0;

            switch (alignment)
            {
                case HorizontalAlignment.Left: break;
                case HorizontalAlignment.Center: offset = (width - lineWidth) / 2; break;
                case HorizontalAlignment.Right: offset = width - lineWidth; break;
            }

            return IsMonospace ? offset / _charSpacing * _charSpacing : offset;
        }

        public int MeasureVerticalOffset(VerticalAlignment alignment, string text, int height)
        {
            var textHeight = MeasureHeight(text);
            var lineHeight = DefaultLineHeight + LineHeight;
            var offset = 0;

            switch (alignment)
            {
                case VerticalAlignment.Top: break;
                case VerticalAlignment.Center: offset = (height - textHeight) / 2; break;
                case VerticalAlignment.Bottom: offset = height - textHeight; break;
            }

            return IsMonospace && lineHeight != 0 ? offset / lineHeight * lineHeight : offset;
        }

        public string WordWrap(string text, int maxWidth)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            _builder.Clear();

            var words = Regex.Split(text, @"(\s)");
            var width = 0;

            for (int i = 0; i < words.Length; i++)
            {
                var wordWidth = MeasureWidth(words[i]);

                if (wordWidth + width > maxWidth)
                {
                    _builder.Append('\n');
                    width = 0;
                    if (words[i].Equals(" "))
                        continue;
                }

                if (wordWidth > maxWidth)
                {
                    var start = 0;

                    for (int j = 1; j < words[i].Length; j++)
                    {
                        var subText = words[i].Substring(start, j - start - 1);

                        if (i - start > 1 && MeasureWidth(subText) > maxWidth)
                        {
                            _builder.Append(subText);
                            _builder.Append('\n');
                            start = i - 1;
                        }
                    }

                    var remaining = words[i].Substring(start, words[i].Length - start);
                    _builder.Append(remaining);
                    width += MeasureWidth(remaining);
                }
                else
                {
                    _builder.Append(words[i]);
                    width += wordWidth;
                }
            }

            return _builder.ToString();
        }

        public BitmapFont Clone()
        {
            return new BitmapFont
            {
                Characters = new Dictionary<int, BitmapCharacter>(Characters),
                Spacing = Spacing,
                DefaultLineHeight = DefaultLineHeight,
                LineHeight = LineHeight
            };
        }
    }

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    public enum VerticalAlignment
    {
        Top,
        Center,
        Bottom
    }
}
