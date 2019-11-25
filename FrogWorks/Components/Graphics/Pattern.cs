using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Pattern : GraphicsComponent
    {
        private Point _size, _mapSize, _remaining;

        public Texture Texture { get; protected set; }

        public Point Size
        {
            get { return _size; }
            set
            {
                value = value.Abs();
                if (_size == value) return;
                _size = value;
                Resize();
            }
        }

        public int Width
        {
            get { return Size.X; }
            set { Size = new Point(value, Size.Y); }
        }

        public int Height
        {
            get { return Size.Y; }
            set { Size = new Point(Size.X, value); }
        }

        public Pattern(Texture texture)
            : this(texture, texture.Size, true)
        {
        }

        public Pattern(Texture texture, Point size)
            : this(texture, size, true)
        {
        }

        public Pattern(Texture texture, int width, int height)
            : this(texture, new Point(width, height))
        {
        }

        protected Pattern(Texture texture, Point size, bool isEnabled) 
            : base(isEnabled)
        {
            Texture = texture;
            Size = size;
        }

        protected override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < _mapSize.X * _mapSize.Y; i++)
            {
                var x = i % _mapSize.X;
                var y = i / _mapSize.X;
                var origin = Origin - new Vector2(x * Texture.Width, y * Texture.Height);
                var width = x < _mapSize.X - 1 || _remaining.X == 0 ? Texture.Width : _remaining.X;
                var height = y < _mapSize.Y - 1 || _remaining.Y == 0 ? Texture.Height : _remaining.Y;
                var bounds = new Rectangle(Texture.Bounds.X, Texture.Bounds.Y, width, height);

                Texture.Draw(batch, DrawPosition, bounds, origin, Scale, Angle, Color * Opacity.Clamp(0f, 1f), SpriteEffects);
            }
        }

        private void Resize()
        {
            _mapSize.X = _size.X / Texture.Width;
            _mapSize.Y = _size.Y / Texture.Height;
            _remaining.X = _size.X - _mapSize.X * Texture.Width;
            _remaining.Y = _size.Y - _mapSize.Y * Texture.Height;

            if (_remaining.X > 0) _mapSize.X++;
            if (_remaining.Y > 0) _mapSize.Y++;
        }
    }
}
