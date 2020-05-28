using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public sealed class Circle : Shape
    {
        float _radius;

        public float Radius
        {
            get { return _radius; }
            set
            {
                value = value.Abs();

                if (_radius != value)
                {
                    _radius = value;
                    MarkAsDirty();
                }
            }
        }

        public override Vector2 Center
        {
            get { return Position + Vector2.One * Radius; }
            set { Position = value - Vector2.One * Radius; }
        }

        public Circle(Vector2 position, float radius)
            : base(position)
        {
            _radius = radius.Abs();
        }

        public Circle(float x, float y, float radius)
            : this(new Vector2(x, y), radius)
        {
        }

        public override bool Contains(Vector2 point)
        {
            return (point - Center).LengthSquared() < Radius * Radius;
        }

        public override Vector2 GetClosestPointOnPoint(Vector2 point)
        {
            return Center + Vector2.Normalize(point - Center) * Radius;
        }

        public override Vector2[] GetVertices()
        {
            return new[] { Center };
        }

        public override void Draw(RendererBatch batch, Color color, Color fill)
        {
            Draw(batch, color, fill, 16);
        }

        public void Draw(RendererBatch batch, Color color, Color fill, int segments)
        {
            batch.DrawPrimitives(p =>
            {
                if (fill != default)
                    p.FillCircle(Center, Radius, fill, segments);

                p.DrawCircle(Center, Radius, color, segments);
            });
        }

        public override Shape Clone()
        {
            return new Circle(Position, Radius);
        }

        protected override Rectangle RecalculateBounds()
        {
            return new Rectangle(Position.ToPoint(), (Vector2.One * _radius * 2f).ToPoint());
        }
    }
}
