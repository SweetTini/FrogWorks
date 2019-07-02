using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class TiledImage : Image
    {
        private int _width, _height;

        public override Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        public int Width
        {
            get { return _width; }
            set
            {
                value = Math.Abs(value);

                if (value == _width) return;
                _width = value;
                UpdateDimensions();
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
                UpdateDimensions();
            }
        }

        public int Columns { get; private set; }

        public int Rows { get; private set; }

        public int RemainingWidth { get; private set; }

        public int RemainingHeight { get; private set; }

        public TiledImage(Texture texture, bool isEnabled)
            : this(texture, texture.Width, texture.Height, isEnabled)
        {
        }

        public TiledImage(Texture texture, int width, int height, bool isEnabled)
            : base(texture, isEnabled)
        {
            Width = width;
            Height = height;
        }

        public override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < Columns * Rows; i++)
            {
                var x = i % Columns;
                var y = i / Columns;

                var origin = Origin - new Vector2(x * Texture.Width, y * Texture.Height);
                var width = x < Columns - 1 || RemainingWidth == 0 ? Texture.Width : RemainingWidth;
                var height = y < Rows - 1 || RemainingHeight == 0 ? Texture.Height : RemainingHeight;
                var bounds = new Rectangle(Texture.Bounds.X, Texture.Bounds.Y, width, height);

                Texture.Draw(batch, DrawPosition, bounds, origin, Scale, Angle, Color, SpriteEffects); 
            }
        }

        private void UpdateDimensions()
        {
            Columns = _width / Texture.Width;
            Rows = _height / Texture.Height;
            RemainingWidth = _width - Columns * Texture.Width;
            RemainingHeight = _height - Rows * Texture.Height;
            if (RemainingWidth > 0) Columns++;
            if (RemainingHeight > 0) Rows++;
        }
    }
}
