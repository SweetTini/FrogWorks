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

        public override bool Collide(Collider other)
        {
            if (!Equals(other) && IsCollidable && other.IsCollidable)
            {
                if (other is ShapeCollider<Shape>)
                    return Collide((other as ShapeCollider<Shape>).Shape);
                else if (other is TileMapCollider)
                    return (other as TileMapCollider).Collide(Shape);
            }

            return false;
        }

        public override bool Collide(Collider other, out Manifold hit)
        {
            hit = new Manifold();

            if (!Equals(other) && IsCollidable && other.IsCollidable)
            {
                if (other is ShapeCollider<Shape>)
                    return Collide((other as ShapeCollider<Shape>).Shape, out hit);
                else if (other is TileMapCollider)
                    return (other as TileMapCollider).Collide(Shape, out hit);
            }

            return false;
        }

        internal override void OnAdded(Entity entity)
        {
            base.OnAdded(entity);
            UpdateShapePosition();
            Entity?.Scene?.ColliderTree.Insert(this);
        }

        internal override void OnRemoved()
        {
            Entity?.Scene?.ColliderTree.Remove(this);
            base.OnRemoved();
            UpdateShapePosition();
        }

        internal override void OnEntityAdded(Entity entity)
        {
            base.OnEntityAdded(entity);
            UpdateShapePosition();
            Entity?.Scene?.ColliderTree.Insert(this);
        }

        internal override void OnEntityRemoved(Entity entity)
        {
            Entity?.Scene?.ColliderTree.Remove(this);
            base.OnEntityRemoved(entity);
            UpdateShapePosition();
        }

        internal override void OnTransformed()
        {
            UpdateShapePosition();
            Entity?.Scene?.ColliderTree.Update(this);
            base.OnTransformed();
        }

        private void UpdateShapePosition()
        {
            if (Shape != null)
                Shape.Position = AbsolutePosition;
        }
    }
}
