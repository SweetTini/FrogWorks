using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class ShapeCollider : Collider, IAABBContainer
    {
        protected internal abstract Shape Shape { get; }

        public AABB Bounds
        {
            get
            {
                var upper = Shape.Bounds.Location.ToVector2();
                var lower = upper + Shape.Bounds.Size.ToVector2();
                return new AABB(upper, lower);
            }
        }

        public override Vector2 Upper
        {
            get { return Shape.Bounds.Location.ToVector2(); }
            set
            {
                var offset = Shape.Bounds.Location.ToVector2() - AbsolutePosition;
                AbsolutePosition = value - offset;
            }
        }

        public override Vector2 Lower
        {
            get { return (Shape.Bounds.Location + Shape.Bounds.Size).ToVector2(); }
            set
            {
                var offset = (Shape.Bounds.Location + Shape.Bounds.Size).ToVector2() - AbsolutePosition;
                AbsolutePosition = value - offset;
            }
        }

        public override void DebugDraw(RendererBatch batch, Color color, bool fill = false)
            => Shape.Draw(batch, color, fill);

        public override bool Contains(Vector2 point) => IsCollidable && Shape.Contains(point);

        public override bool Collide(Ray ray)
        {
            Raycast hit;
            return IsCollidable && ray.Cast(Shape, out hit);
        }

        public override bool Collide(Shape shape) => IsCollidable && Shape.Collide(shape);

        public override bool Collide(Collider other)
        {
            if (IsCollidable)
            {
                if (other is ShapeCollider)
                    return Shape.Collide((other as ShapeCollider).Shape);
            }

            return false;
        }

        protected override void OnAdded() => IncludeIntoTree();

        protected override void OnRemoved() => ExcludeFromTree();

        protected override void OnEntityAdded() => IncludeIntoTree();

        protected override void OnEntityRemoved() => ExcludeFromTree();

        protected override void OnLayerAdded() => IncludeIntoTree();

        protected override void OnLayerRemoved() => ExcludeFromTree();

        protected override void OnTransformed() => UpdateTree();

        private void IncludeIntoTree() => Scene?.ColliderTree.Insert(this);

        private void ExcludeFromTree() => Scene?.ColliderTree.Remove(this);

        private void UpdateTree() => Scene?.ColliderTree.Update(this);
    }
}
