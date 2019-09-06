using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FrogWorks
{
    public class IndexMapCollider : MapCollider<int>
    {
        public IndexMapCollider(int columns, int rows, int cellWidth, int cellHeight, float x = 0, float y = 0)
            : base(columns, rows, cellWidth, cellHeight, x, y) { }

        public bool Collide(Vector2 point, int index) 
            => Validate(At(point), index, (p, i) => CheckShape(p, s => s.Contains(point)));

        public bool Collide(Vector2 point, IEnumerable<int> indices)
            => Validate(At(point), indices, (p, i) => CheckShape(p, s => s.Contains(point)));

        public bool Collide(Ray ray, int index)
            => Validate(At(ray.Position, ray.Endpoint), index, (p, i) => CheckShape(p, s => ray.Cast(s)));

        public bool Collide(Ray ray, IEnumerable<int> indices)
            => Validate(At(ray.Position, ray.Endpoint), indices, (p, i) => CheckShape(p, s => ray.Cast(s)));

        public bool Collide(Shape shape, int index)
            => Validate(At(shape.Bounds), index, (p, i) => CheckShape(p, s => shape.Collide(s)));

        public bool Collide(Shape shape, IEnumerable<int> indices)
            => Validate(At(shape.Bounds), indices, (p, i) => CheckShape(p, s => shape.Collide(s)));

        public bool Collide(Collider collider, int index)
        {
            if (collider == null || Equals(collider) || !collider.IsCollidable) return false;

            return Validate(At(collider.Bounds), index, (p, i)
                => CheckShape(p, s => collider is ShapeCollider && (collider as ShapeCollider).Collide(s)));
        }

        public bool Collide(Collider collider, IEnumerable<int> indices)
        {
            if (collider == null || Equals(collider) || !collider.IsCollidable) return false;

            return Validate(At(collider.Bounds), indices, (p, i)
                => CheckShape(p, s => collider is ShapeCollider && (collider as ShapeCollider).Collide(s)));
        }

        public bool Collide(Entity entity, int index) => Collide(entity?.Collider, index);

        public bool Collide(Entity entity, IEnumerable<int> indices) => Collide(entity?.Collider, indices);

        public override Collider Clone()
        {
            var collider = new IndexMapCollider(Columns, Rows, CellWidth, CellHeight, X, Y);
            collider.Populate(Map.ToArray());
            return collider;
        }

        protected override Shape ShapeOf(Point point)
        {
            if (!Map.IsEmpty(point.X, point.Y))
                return new RectangleF(AbsolutePosition + (point * CellSize).ToVector2(), CellSize.ToVector2());

            return null;
        }
    }
}
