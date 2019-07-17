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

        public override Vector2 Upper
        {
            get { return Shape.Bounds.Location.ToVector2(); }
            set { AbsolutePosition = value - Shape.Bounds.Location.ToVector2(); }
        }

        public override Vector2 Lower
        {
            get { return (Shape.Bounds.Location + Shape.Bounds.Size).ToVector2(); }
            set { AbsolutePosition = value - (Shape.Bounds.Location + Shape.Bounds.Size).ToVector2(); }
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
            return IsCollidable && Shape.Contains(point);
        }

        public override bool CastRay(Ray ray, out Raycast hit)
        {
            hit = new Raycast(ray);

            return IsCollidable && ray.Cast(Shape, out hit);
        }

        public override bool Collide(Shape other)
        {
            return IsCollidable && Shape.Collide(other);
        }

        public override bool Collide(Shape other, out Manifold hit)
        {
            hit = new Manifold();

            return IsCollidable && Shape.Collide(other, out hit);
        }

        internal override void OnAdded(Entity entity)
        {
            base.OnAdded(entity);
            Entity?.Scene?.ColliderTree.Insert(this);
        }

        internal override void OnRemoved()
        {
            Entity?.Scene?.ColliderTree.Remove(this);
            base.OnRemoved();
        }

        internal override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);
            Entity?.Scene?.ColliderTree.Insert(this);
        }

        internal override void OnEntityRemoved(Entity entity)
        {
            Entity?.Scene?.ColliderTree.Remove(this);
            base.OnEntityRemoved(entity);
        }

        internal override void OnTranslated(Vector2 offset)
        {
            if (Shape != null)
                Shape.Position = AbsolutePosition;
            Entity?.Scene?.ColliderTree.Update(this);
            base.OnTranslated(offset);
        }

        internal override void OnTransformed()
        {
            Entity?.Scene?.ColliderTree.Update(this);
            base.OnTransformed();
        }
    }
}
