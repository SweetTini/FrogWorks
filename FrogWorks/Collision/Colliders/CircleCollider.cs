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
                OnInternalTransformed();
            }
        }

        public override Vector2 Size
        {
            get { return Vector2.One * _radius * 2f; }
            set { Radius = (value.Abs().X > value.Abs().Y ? value.X : value.Y) / 2f; }
        }

        public CircleCollider(float radius, float x = 0f, float y = 0f)
            : base(new Vector2(x, y))
        {
            _radius = Math.Abs(radius);
        }

        public override Collider Clone() => new CircleCollider(Radius, X, Y);
    }
}
