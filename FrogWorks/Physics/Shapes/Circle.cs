using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public sealed class Circle : Shape
    {
        float _radius;

        public override Vector2 Center
        {
            get { return Position + Vector2.One * Radius; }
            set { Position = value - Vector2.One * Radius; }
        }

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

        public Circle(float x, float y, float radius)
            : this(new Vector2(x, y), radius)
        {
        }

        public Circle(Vector2 position, float radius)
            : base(position)
        {
            _radius = radius;
        }

        protected override Rectangle RecalculateBounds()
        {
            var position = Position.ToPoint();
            var size = (Vector2.One * _radius * 2f).ToPoint();
            return new Rectangle(position, size);
        }

        public override bool Contains(Vector2 point)
        {
            return (point - Center).LengthSquared() < Radius * Radius;
        }

        public override void Draw(
            RendererBatch batch,
            Color strokeColor,
            Color? fillColor = null)
        {
            batch.DrawPrimitives(p =>
            {
                if (fillColor.HasValue)
                    p.FillCircle(Center, _radius, fillColor.Value);

                p.DrawCircle(Center, _radius, strokeColor);
                p.DrawDot(Center, strokeColor);
            });
        }

        public override Shape Clone()
        {
            return new Circle(Position, _radius);
        }

        internal override Vector2[] GetFocis()
        {
            return new Vector2[] { Center };
        }

        internal override Vector2[] GetAxes(Vector2[] focis)
        {
            return new Vector2[] { Vector2.Zero };
        }

        internal override void Project(Vector2 axis, out float min, out float max)
        {
            var dotProd = Vector2.Dot(Center, axis);

            min = dotProd - Radius;
            max = dotProd + Radius;
        }
    }
}
