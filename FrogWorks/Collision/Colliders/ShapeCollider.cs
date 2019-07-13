using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class ShapeCollider<T> : Collider
        where T : Shape
    {
        protected internal T Shape { get; protected set; }

        public override bool Contains(Vector2 point)
        {
            return Shape.Contains(point);
        }

        public override bool CastRay(Ray ray, out Raycast hit)
        {
            return ray.Cast(Shape, out hit);
        }

        public override bool Collide(Shape shape)
        {
            return Shape.Collide(shape);
        }

        public override bool Collide(Shape shape, out Manifold hit)
        {
            return Shape.Collide(shape, out hit);
        }

        internal override void OnTranslated(Vector2 position, Vector2 lastPosition)
        {
            if (Shape != null)
                Shape.Position = AbsolutePosition;

            base.OnTranslated(position, lastPosition);
        }
    }
}
