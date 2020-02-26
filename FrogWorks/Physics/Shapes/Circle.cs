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
            });
        }

        public override Shape Clone()
        {
            return new Circle(Position, _radius);
        }
    }
}
