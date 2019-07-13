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
        {
            get
            {
                var location = Position.Round().ToPoint();
                var size = (Vector2.One * _radius * 2f).Round().ToPoint();
                return new Rectangle(location, size);
            }
        }

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Circle(float centerX, float centerY, float radius)
            : this(new Vector2(centerX, centerY), radius)
        {
        }

        public override void Draw(RendererBatch batch, Color color, bool fill = false)
        {
            batch.DrawPrimitives((primitive) =>
            {
                if (fill) primitive.FillCircle(Center, Radius, color);
                else primitive.DrawCircle(Center, Radius, color);
            });
        }

        public override bool Contains(Vector2 point)
        {
            return (point - Center).LengthSquared() < Radius * Radius;
        }

        public override Shape Clone()
        {
            return new Circle(Center, Radius);
        }

        public override Proxy ToProxy()
        {
            return new Proxy(new[] { Center }, Radius);
        }
    }
}
