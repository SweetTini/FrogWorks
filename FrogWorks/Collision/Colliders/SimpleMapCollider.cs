using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class SimpleMapCollider : MapCollider<bool>
    {
        public SimpleMapCollider(int columns, int rows, int cellWidth, int cellHeight, float x = 0, float y = 0)
            : base(columns, rows, cellWidth, cellHeight, x, y) { }

        public override bool Contains(Vector2 point)
        {
            return CheckCells(GetCells(point), cs => cs.Contains(point));
        }

        public override bool Collide(Ray ray)
        {
            Raycast hit;
            return CheckCells(GetCells(ray.Position, ray.Endpoint), cs => ray.Cast(cs, out hit));
        }

        public override bool Collide(Shape shape)
        {
            return CheckCells(GetCells(shape.Bounds), cs => shape.Collide(cs));
        }

        public override bool Collide(Collider other)
        {
            return CheckCells(GetCells(other.Bounds), (cs) => 
            {
                if (!Equals(other) && other.IsCollidable)
                    if (other is ShapeCollider)
                        return (other as ShapeCollider).Collide(cs);

                return false;
            });
        }

        public override Collider Clone()
        {
            return new SimpleMapCollider(Columns, Rows, CellWidth, CellHeight, X, Y)
            {
                Map = new Map<bool>(Map.ToArray(), Map.Empty)
            };
        }
    }
}
