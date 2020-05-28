using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public sealed class Box : Shape
    {
        Vector2 _size;

        public Vector2 Size
        {
            get { return _size; }
            set
            {
                value = value.Abs();

                if (_size != value)
                {
                    _size = value;
                    MarkAsDirty();
                }
            }
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

        public Vector2 Min => Position;

        public Vector2 Max => Position + Size;

        public override Vector2 Center 
        {
            get { return Position + Size * .5f; }
            set { Position = value - Size * .5f; }
        }

        public Box(Vector2 position, Vector2 size)
            : base(position)
        {
            _size = size.Abs();
        }

        public Box(float x, float y, float width, float height)
            : this(new Vector2(x, y), new Vector2(width, height))
        {
        }

        public override bool Contains(Vector2 point)
        {
            return Min.X <= point.X && point.X <= Max.X 
                && Min.Y <= point.Y && point.Y <= Max.Y;
        }

        public bool Overlaps(Shape shape, bool nearestAxis, out CollisionResult result)
        {
            result = default;
            return false;
        }

        public override Vector2[] GetVertices()
        {
            return new Vector2[]
            {
                Position,
                Position + Size * Vector2.UnitX,
                Position + Size,
                Position + Size * Vector2.UnitY
            };
        }

        public override void Draw(RendererBatch batch, Color color, Color fill)
        {
            batch.DrawPrimitives(p => 
            {
                if (fill != default)
                    p.FillRectangle(Position, Size, fill);

                p.DrawRectangle(Position, Size, color);
            });
        }

        public override Shape Clone()
        {
            return new Box(Position, Size);
        }

        protected override Rectangle RecalculateBounds()
        {
            return new Rectangle(Position.ToPoint(), Size.ToPoint());
        }
    }
}
