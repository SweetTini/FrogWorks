using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class Circle : Shape
    {
        private float _radius;

        public Vector2 Center
        {
            get { return Position + Vector2.One * _radius; }
            set { Position = value - Vector2.One * _radius; }
        }

        public float CenterX
        {
            get { return X + _radius; }
            set { X = value - _radius; }
        }

        public float CenterY
        {
            get { return Y + _radius; }
            set { Y = value - _radius; }
        }

        public float Radius
        {
            get { return _radius; }
            set { _radius = Math.Abs(value); }
        }

        public override Rectangle Bounds
            => new Rectangle(Position.Round().ToPoint(), (Vector2.One * _radius * 2f).Round().ToPoint());

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
            : this(new Vector2(x, y), radius) { }

        public override void Draw(RendererBatch batch, Color stroke, Color? fill = null)
        {
            batch.DrawPrimitives((primitive) =>
            {
                if (fill.HasValue)
                    primitive.FillCircle(Center, Radius, fill.Value);
                primitive.DrawCircle(Center, Radius, stroke);
            });
        }

        public override bool Contains(Vector2 point) => (point - Center).LengthSquared() < Radius * Radius;

        public override Shape Clone() => new Circle(Center, Radius);

        public override Proxy ToProxy() => new Proxy(new[] { Center }, Radius);
    }
}
