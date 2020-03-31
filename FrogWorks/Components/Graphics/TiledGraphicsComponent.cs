using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public abstract class TiledGraphicsComponent : Component
    {
        Vector2 _position;

        public Texture Texture { get; protected set; }

        public Point TileSize { get; protected set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        Rectangle DrawRegion
        {
            get
            {
                var tileSize = TileSize.ToVector2();
                var camera = Layer?.Camera ?? Scene?.Camera;
                var min = camera?.Min ?? Vector2.Zero;
                var max = camera?.Max ?? Runner.Application.ActualSize.ToVector2();

                min = (min - DrawPosition).Divide(tileSize).Floor();
                max = (max + DrawPosition).Divide(tileSize).Ceiling();

                return new Rectangle(
                    min.ToPoint(),
                    (max - min).ToPoint());
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnTransformedInternally();
                }
            }
        }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        public Vector2 DrawPosition
        {
            get { return Position + (Parent?.Position ?? Vector2.Zero); }
            set { Position = value - (Parent?.Position ?? Vector2.Zero); }
        }

        public Color Color { get; set; } = Color.White;

        public float Opacity { get; set; } = 1f;

        public SpriteEffects SpriteEffects { get; set; }

        public bool FlipHorizontally
        {
            get { return SpriteEffects.HasFlag(SpriteEffects.FlipHorizontally); }
            set
            {
                SpriteEffects = value
                    ? (SpriteEffects | SpriteEffects.FlipHorizontally)
                    : (SpriteEffects & ~SpriteEffects.FlipHorizontally);
            }
        }

        public bool FlipVertically
        {
            get { return SpriteEffects.HasFlag(SpriteEffects.FlipVertically); }
            set
            {
                SpriteEffects = value
                    ? (SpriteEffects | SpriteEffects.FlipVertically)
                    : (SpriteEffects & ~SpriteEffects.FlipVertically);
            }
        }

        public bool WrapHorizontally { get; set; }

        public bool WrapVertically { get; set; }

        protected TiledGraphicsComponent(bool isEnabled)
            : base(isEnabled, true)
        {
        }

        protected sealed override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < DrawRegion.Width * DrawRegion.Height; i++)
            {
                var x = DrawRegion.Left + (i % DrawRegion.Width);
                var y = DrawRegion.Top + (i / DrawRegion.Width);
                var position = DrawPosition + new Vector2(x * TileWidth, y * TileHeight);

                GetTile(x, y)?.Draw(batch, position, Vector2.Zero, Vector2.One, 0f,
                                    Color * Opacity.Clamp(0f, 1f), SpriteEffects);
            }
        }

        protected abstract Texture GetTile(int x, int y);
    }
}
