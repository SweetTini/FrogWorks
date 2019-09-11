using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class RectangleF : Shape
    {
        private Vector2 _size;

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

        public Vector2 Upper => Position;

        public Vector2 Lower => Position + Size;

        public override Rectangle Bounds 
            => new Rectangle(Position.Round().ToPoint(), Size.Round().ToPoint());

        public RectangleF(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public RectangleF(float x, float y, float width, float height)
            : this(new Vector2(x, y), new Vector2(width, height)) { }

        public override void Draw(RendererBatch batch, Color stroke, Color? fill = null)
        {
            batch.DrawPrimitives((primitive) =>
            {
                if (fill.HasValue)
                    primitive.FillRectangle(Position, Size, fill.Value);
                primitive.DrawRectangle(Position, Size, stroke);
            });
        }

        public override bool Contains(Vector2 point)
        {
            return Upper.X <= point.X && point.X < Lower.X 
                && Upper.Y <= point.Y && point.Y < Lower.Y;
        }

        public override Shape Clone() => new RectangleF(Position, Size);

        public override Proxy ToProxy() => new Proxy(ToVertices());

        public Vector2[] ToVertices()
        {
            return new[]
            {
                Upper, new Vector2(Lower.X, Upper.Y),
                Lower, new Vector2(Upper.X, Lower.Y),
            };
        }
    }
}
