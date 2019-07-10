using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public struct Circle : IShape
    {
        private float _radius;

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

        public Rectangle Bounds => new Rectangle(Position.Round().ToPoint(), (Vector2.One * _radius * 2f).Round().ToPoint());

        public Circle(Vector2 center, float radius)
            : this()
        {
            Center = center;
            Radius = radius;
        }

        public Circle(float centerX, float centerY, float radius)
            : this(new Vector2(centerX, centerY), radius)
        {
        }

        public void Draw(RendererBatch batch, Color color, bool fill = false)
        {
            var shape = this;

            batch.DrawPrimitives((primitive) =>
            {
                if (fill) primitive.FillCircle(shape.Center, shape.Radius, color);
                else primitive.DrawCircle(shape.Center, shape.Radius, color);
            });
        }

        public bool Contains(Vector2 point)
        {
            return (point - Center).LengthSquared() < Radius * Radius;
        }

        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }
    }
}
