﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace FrogWorks
{
    public class BitmapFont
    {
        StringBuilder _builder;
        int _charSpacing;

        public BitmapCharacter this[int ascii]
        {
            get
            {
                return Characters.ContainsKey(ascii)
                    ? Characters[ascii] : null;
            }
        }

        protected internal Dictionary<int, BitmapCharacter> Characters
        {
            get; private set;
        }

        public int Spacing { get; set; }

        public int DefaultLineHeight { get; internal set; }

        public int LineHeight { get; set; }

        internal BitmapFont()
        {
            _builder = new StringBuilder();
            Characters = new Dictionary<int, BitmapCharacter>();
        }

        public BitmapFont(Texture texture, int charWidth, int charHeight, string charSet)
            : this()
        {
            var textures = Texture.Split(texture, charWidth, charHeight);

            for (int i = 0; i < charSet.Length; i++)
            {
                Characters.Add(
                    charSet[i],
                    new BitmapCharacter(
                        textures[i],
                        charSet[i],
                        Point.Zero,
                        charWidth));
            }

            _charSpacing = charWidth;
            DefaultLineHeight = charHeight;
        }

        public void Draw(
            RendererBatch batch,
            string text,
            Point position,
            Point size,
            Vector2? origin = null,
            Vector2? scale = null,
            float angle = 0f,
            Color? color = null,
            SpriteEffects effects = SpriteEffects.None,
            HorizontalAlignment xAlign = HorizontalAlignment.Left,
            VerticalAlignment yAlign = VerticalAlignment.Top,
            bool wordWrap = false)
        {
            Draw(
                batch,
                text,
                position.ToVector2(),
                size.ToVector2(),
                origin,
                scale,
                angle,
                color,
                effects,
                xAlign,
                yAlign,
                wordWrap);
        }

        public void Draw(
            RendererBatch batch,
            string text,
            Vector2 position,
            Vector2 size,
            Vector2? origin = null,
            Vector2? scale = null,
            float angle = 0f,
            Color? color = null,
            SpriteEffects effects = SpriteEffects.None,
            HorizontalAlignment xAlign = HorizontalAlignment.Left,
            VerticalAlignment yAlign = VerticalAlignment.Top,
            bool wordWrap = false)
        {
            if (text.IsNullOrEmpty()) return;
            if (wordWrap) text = WordWrap(text, (int)size.X);

            var offset = MeasureVerticalOffset(yAlign, text, (int)size.Y).ToUnitYF();
            var lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                offset.X = MeasureHorizontalOffset(xAlign, line, (int)size.X);

                for (int j = 0; j < line.Length; j++)
                {
                    BitmapCharacter character;

                    if (Characters.TryGetValue(line[j], out character))
                    {
                        var kerning = 0;
                        if (j < line.Length - 1)
                            character.Kernings.TryGetValue(line[j + 1], out kerning);
                        var charOrigin = (origin ?? Vector2.Zero)
                            - (offset + character.Offset.ToVector2());

                        character.Texture.Draw(
                            batch,
                            position,
                            charOrigin,
                            scale ?? Vector2.One,
                            angle,
                            color ?? Color.White,
                            effects);

                        offset.X += kerning + character.Spacing + Spacing;
                    }
                }

                offset.Y += DefaultLineHeight + LineHeight;
            }
        }

        public void Configure(int ascii, int offsetX, int offsetY, int spacing)
        {
            BitmapCharacter character;

            if (Characters.TryGetValue(ascii, out character))
            {
                character.Offset = new Point(offsetX, offsetY);
                character.Spacing = spacing;
            }
        }

        public Point Measure(char ascii)
        {
            BitmapCharacter character;

            if (Characters.TryGetValue(ascii, out character))
            {
                return new Point(
                    character.Spacing + Spacing,
                    DefaultLineHeight + LineHeight);
            }

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

            return lines * (DefaultLineHeight + LineHeight);
        }

        public int MeasureHorizontalOffset(
            HorizontalAlignment alignment,
            string line,
            int width)
        {
            var lineWidth = MeasureWidth(line);
            var offset = 0;

            switch (alignment)
            {
                case HorizontalAlignment.Left:
                    break;
                case HorizontalAlignment.Center:
                    offset = (int)Math.Round((width - lineWidth) / 2f);
                    break;
                case HorizontalAlignment.Right:
                    offset = width - lineWidth;
                    break;
            }

            return offset;
        }

        public int MeasureVerticalOffset(
            VerticalAlignment alignment,
            string text,
            int height)
        {
            var textHeight = MeasureHeight(text);
            var offset = 0;

            switch (alignment)
            {
                case VerticalAlignment.Top:
                    break;
                case VerticalAlignment.Center:
                    offset = (int)Math.Round((height - textHeight) / 2f);
                    break;
                case VerticalAlignment.Bottom:
                    offset = height - textHeight;
                    break;
            }

            return offset;
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
                    if (words[i] == "\n")
                        width = 0;
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
                _charSpacing = _charSpacing,
                Spacing = Spacing,
                DefaultLineHeight = DefaultLineHeight,
                LineHeight = LineHeight
            };
        }
    }

    public class BitmapCharacter
    {
        Dictionary<int, int> _kernings;

        public Texture Texture { get; internal set; }

        public int Ascii { get; internal set; }

        public Point Offset { get; internal set; }

        public int Spacing { get; internal set; }

        public ReadOnlyDictionary<int, int> Kernings { get; }

        internal BitmapCharacter()
        {
            _kernings = new Dictionary<int, int>();
            Kernings = new ReadOnlyDictionary<int, int>(_kernings);
        }

        internal BitmapCharacter(
            Texture texture,
            int ascii,
            Point offset = default,
            int spacing = 0)
            : this()
        {
            Texture = texture;
            Ascii = ascii;
            Offset = offset;
            Spacing = spacing;
        }

        internal BitmapCharacter(
            Texture texture,
            int ascii,
            int offsetX = 0,
            int offsetY = 0,
            int spacing = 0)
            : this(texture, ascii, new Point(offsetX, offsetY), spacing)
        {
        }

        internal void AddOrUpdateKerning(int ascii, int spacing)
        {
            if (_kernings.ContainsKey(ascii))
                _kernings[ascii] = spacing;
            else _kernings.Add(ascii, spacing);
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
