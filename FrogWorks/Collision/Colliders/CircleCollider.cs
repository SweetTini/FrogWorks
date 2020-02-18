using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class CircleCollider : ShapeCollider
    {
        private float _radius;

        protected internal override Shape Shape 
            => new Circle(AbsolutePosition + Vector2.One * _radius, _radius);

        public float Radius
        {
            get { return _radius; }
            set
            {
                value = Math.Abs(value);
                if (value == _radius) return;
                _radius = value;
                OnTransformedInternally();
            }
        }

        public override Vector2 Size
        {
            get { return Vector2.One * _radius * 2f; }
            set { Radius = (value.Abs().X > value.Abs().Y ? value.X : value.Y) / 2f; }
        }

        public CircleCollider(float radius)
            : this(radius, Vector2.Zero)
        {
            _radius = Math.Abs(radius);
        }

        public CircleCollider(float radius, Vector2 offset)
            : base(offset)
        {
            _radius = Math.Abs(radius);
        }

        public CircleCollider(float radius, float offsetX, float offsetY)
            : this(radius, new Vector2(offsetX, offsetY))
        {
        }

        public override Collider Clone()
        {
            return new CircleCollider(Radius, Position);
        }
    }
}
