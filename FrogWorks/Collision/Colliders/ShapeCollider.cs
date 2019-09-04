using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class ShapeCollider : Collider, IAABBContainer
    {
        protected internal abstract Shape Shape { get; }

        public sealed override Vector2 Upper
        {
            get { return Shape.Bounds.Location.ToVector2(); }
            set
            {
                var offset = Shape.Bounds.Location.ToVector2() - AbsolutePosition;
                AbsolutePosition = value - offset;
            }
        }

        public sealed override Vector2 Lower
        {
            get { return (Shape.Bounds.Location + Shape.Bounds.Size).ToVector2(); }
            set
            {
                var offset = (Shape.Bounds.Location + Shape.Bounds.Size).ToVector2() - AbsolutePosition;
                AbsolutePosition = value - offset;
            }
        }

        public AABB AabbBounds => new AABB(Upper, Lower);

        protected ShapeCollider(Vector2 position)
            : base(position) { }

        public sealed override void DebugDraw(RendererBatch batch, Color color, bool fill = false)
            => Shape.Draw(batch, color, fill);

        public sealed override bool Contains(Vector2 point) => IsCollidable && Shape.Contains(point);

        public sealed override bool Collide(Ray ray)
        {
            Raycast hit;
            return IsCollidable && ray.Cast(Shape, out hit);
        }

        public sealed override bool Collide(Shape shape) => IsCollidable && shape.Collide(Shape);

        public sealed override bool Collide(Collider other)
        {
            if (!Equals(other) && IsCollidable && other.IsCollidable)
            {
                if (other is ShapeCollider) return (other as ShapeCollider).Collide(Shape);
                if (other is SimpleMapCollider) return (other as SimpleMapCollider).Collide(Shape);
                if (other is IndexMapCollider) return (other as IndexMapCollider).Collide(Shape);
            }

            return false;
        }

        protected sealed override void OnAdded() => IncludeIntoTree();

        protected sealed override void OnRemoved() => ExcludeFromTree();

        protected sealed override void OnEntityAdded() => IncludeIntoTree();

        protected sealed override void OnEntityRemoved() => ExcludeFromTree();

        protected sealed override void OnLayerAdded() => IncludeIntoTree();

        protected sealed override void OnLayerRemoved() => ExcludeFromTree();

        protected sealed override void OnTransformed() => Scene?.Colliders.Update(this);

        private void IncludeIntoTree() => Scene?.Colliders.Insert(this);

        private void ExcludeFromTree() => Scene?.Colliders.Remove(this);
    }
}
