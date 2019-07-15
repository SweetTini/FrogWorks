using System;
using Microsoft.Xna.Framework;

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

        public override Vector2 Size
        {
            get { return Vector2.One * 2f * Shape.Radius; }
            set
            {
                value = value.Abs();
                var average = (value.X + value.Y) / 2f;
                Radius = average / 2f;
            }
        }

        public CircleCollider(float radius, float offsetX = 0f, float offsetY = 0f)
            : base()
        {
            Shape = new Circle(Vector2.Zero, Math.Abs(radius));
            Position = new Vector2(offsetX, offsetY);
        }

        public override Collider Clone()
        {
            return new CircleCollider(Radius, X, Y);
        }
    }
}
