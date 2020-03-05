using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class ShapeCollider : Collider
    {
        protected internal abstract Shape Shape { get; }

        protected ShapeCollider(Vector2 position)
            : base(position)
        {
        }

        public sealed override bool Contains(Vector2 point)
        {
            return base.Contains(point)
                && Shape.Contains(point);
        }

        public sealed override bool Raycast(Vector2 start, Vector2 end, out Raycast hit)
        {
            return base.Raycast(start, end, out hit)
                && Shape.Raycast(start, end, out hit);
        }

        public sealed override bool Overlaps(Shape shape)
        {
            return base.Overlaps(shape)
                && Shape.Overlaps(shape);
        }

        public sealed override bool Overlaps(Shape shape, out Manifold hit)
        {
            return base.Overlaps(shape, out hit)
                && Shape.Overlaps(shape, out hit);
        }

        public sealed override bool Overlaps(Collider collider)
        {
            if (base.Overlaps(collider))
            {
                if (collider is ShapeCollider)
                    return Overlaps((collider as ShapeCollider).Shape);
            }

            return false;
        }

        public sealed override bool Overlaps(Collider collider, out CollisionResult result)
        {
            if (base.Overlaps(collider, out result))
            {
                if (collider is ShapeCollider)
                {
                    Manifold hit;

                    if (Overlaps((collider as ShapeCollider).Shape, out hit))
                    {
                        result.Add(hit);
                        return true;
                    }
                }
            }

            return false;
        }

        public sealed override void Draw(RendererBatch batch, Color color)
        {
            Shape.Draw(batch, color);
        }

        protected sealed override void OnTransformed()
        {
            Shape.Position = AbsolutePosition;
        }
    }
}
