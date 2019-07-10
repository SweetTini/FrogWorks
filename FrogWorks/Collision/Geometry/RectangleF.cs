using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public struct RectangleF : IShape
    {
        private Vector2 _size;

        public Vector2 Position { get; set; }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.X; }
            set { Position = new Vector2(Position.X, value); }
        }

        public Vector2 Size
        {
            get { return _size; }
            set { _size = value.Abs(); }
        }

        public float Width
        {
            get { return Size.X; }
            set { Size = new Vector2(value, Size.Y); }
        }

        public float Height
        {
            get { return Size.Y; }
            set { Size = new Vector2(Size.X, value); }
        }

        public float Left => Position.X;

        public float Right => Position.X + Size.X;

        public float Top => Position.Y;

        public float Bottom => Position.Y + Size.Y;

        public Rectangle Bounds => new Rectangle(Position.Round().ToPoint(), Size.Round().ToPoint());

        public RectangleF(Vector2 position, Vector2 size)
            : this()
        {
            Position = position;
            Size = size;
        }

        public RectangleF(float x, float y, float width, float height)
            : this(new Vector2(x, y), new Vector2(width, height))
        {
        }

        public void Draw(RendererBatch batch, Color color, bool fill = false)
        {
            var shape = this;

            batch.DrawPrimitives((primitive) =>
            {
                if (fill) primitive.FillRectangle(shape.Position, shape.Size, color);
                else primitive.DrawRectangle(shape.Position, shape.Size, color);
            });
        }

        public bool Contains(Vector2 point)
        {
            return Left <= point.X && point.X < Right && Top <= point.Y && point.Y < Bottom;
        }

        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }
    }
}
