using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public sealed class CircleCollider : ShapeCollider
    {
        Circle _circle;

        protected internal override Shape Shape => _circle;

        public float Radius
        {
            get { return _circle.Radius; }
            set
            {
                value = value.Abs();

                if (_circle.Radius != value)
                {
                    _circle.Radius = value;
                    OnTransformedInternally();
                }
            }
        }

        public override Vector2 Size
        {
            get { return Vector2.One * 2f * _circle.Radius; }
            set
            {
                value = value.Abs();
                Radius = Math.Max(value.X, value.Y) * .5f;
            }
        }

        public override Vector2 Min
        {
            get { return AbsolutePosition; }
            set { AbsolutePosition = value; }
        }

        public override Vector2 Max
        {
            get { return AbsolutePosition + Size; }
            set { AbsolutePosition = value - Size; }
        }

        public CircleCollider(float radius)
            : this(Vector2.Zero, radius)
        {
        }

        public CircleCollider(float x, float y, float radius)
            : this(new Vector2(x, y), radius)
        {
        }

        public CircleCollider(Vector2 position, float radius)
            : base(position)
        {
            _circle = new Circle(AbsolutePosition, radius);
        }

        public override Collider Clone()
        {
            return new CircleCollider(Position, 0f);
        }
    }
}
