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

        public sealed override void Draw(RendererBatch batch, Color stroke, Color? fill = null)
            => Shape.Draw(batch, stroke, fill);

        public sealed override bool Collide(Vector2 point) => IsCollidable && Shape.Contains(point);

        public sealed override bool Collide(Ray ray) => IsCollidable && ray.Cast(Shape);

        public sealed override bool Collide(Shape shape) => IsCollidable && shape.Collide(Shape);

        public sealed override bool Collide(Collider collider)
        {
            var isValid = collider != null
                && !Equals(collider)
                && IsCollidable
                && collider.IsCollidable;

            if (isValid)
            {
                if (collider is ShapeCollider) return (collider as ShapeCollider).Collide(Shape);
                if (collider is SimpleMapCollider) return (collider as SimpleMapCollider).Collide(Shape);
                if (collider is BitFlagMapCollider) return (collider as BitFlagMapCollider).Collide(Shape);
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
