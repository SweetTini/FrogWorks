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

        public sealed override bool Overlaps(Shape shape, out CollisionResult result)
        {
            if (base.Overlaps(shape, out result))
            {
                Manifold hit;

                if (Shape.Overlaps(shape, out hit))
                {
                    result.Add(hit);
                    return true;
                }
            }

            return false;
        }

        public sealed override bool Overlaps(Collider collider)
        {
            if (base.Overlaps(collider))
            {
                if (collider is ShapeCollider)
                {
                    var shape = (collider as ShapeCollider).Shape;
                    return Shape.Overlaps(shape);
                }
                else if (collider is TileMapCollider)
                {
                    var tileMapCollider = collider as TileMapCollider;
                    return tileMapCollider.Overlaps(this);
                }
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
                    var shape = (collider as ShapeCollider).Shape;

                    if (Shape.Overlaps(shape, out hit))
                    {
                        result.Add(hit);
                        return true;
                    }
                }
                else if (collider is TileMapCollider)
                {
                    var tileMapCollider = collider as TileMapCollider;
                    return tileMapCollider.Overlaps(this, out result);
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
