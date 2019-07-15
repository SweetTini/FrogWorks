using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class ShapeCollider<T> : Collider, IAABBContainer
        where T : Shape
    {
        protected internal T Shape { get; protected set; }

        public AABB Bounds
        {
            get
            {
                var upper = Shape.Bounds.Location.ToVector2();
                var lower = upper + Shape.Bounds.Size.ToVector2();
                return new AABB(upper, lower);
            }
        }

        protected ShapeCollider()
            : base()
        {
        }

        public override void Draw(RendererBatch batch, Color color)
        {
            Shape.Draw(batch, color);
        }

        public override bool Contains(Vector2 point)
        {
            return Shape.Contains(point);
        }

        public override bool CastRay(Ray ray, out Raycast hit)
        {
            return ray.Cast(Shape, out hit);
        }

        public override bool Collide(Shape other)
        {
            return Shape.Collide(other);
        }

        public override bool Collide(Shape other, out Manifold hit)
        {
            return Shape.Collide(other, out hit);
        }

        internal override void OnTranslated(Vector2 offset)
        {
            if (Shape != null)
                Shape.Position = AbsolutePosition;
            base.OnTranslated(offset);
        }
    }
}
