using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class CircleCollider : ShapeCollider<Circle>
    {
        public float Radius
        {
            get { return Shape.Radius; }
            set
            {
                value = Math.Abs(value);
                if (value == Shape.Radius) return;
                Shape.Radius = value;
                OnTransformed();
            }
        }

        public override float Width
        {
            get { return Radius * 2f; }
            set { Radius = value / 2f; }
        }

        public override float Height
        {
            get { return Radius * 2f; }
            set { Radius = value / 2f; }
        }

        public CircleCollider(float radius, float offsetX = 0f, float offsetY = 0f)
            : base()
        {
            Shape = new Circle(Vector2.Zero, Math.Abs(Radius));
            Position = new Vector2(offsetX, offsetY);
        }

        public override void Draw(RendererBatch batch, Color color)
        {
            batch.DrawPrimitives((primitive) => primitive.DrawCircle(Position + Vector2.One * Radius, Radius, color));
        }

        public override Collider Clone()
        {
            return new CircleCollider(Radius, X, Y);
        }
    }
}
