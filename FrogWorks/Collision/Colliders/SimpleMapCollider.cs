using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class SimpleMapCollider : MapCollider<bool>
    {
        public SimpleMapCollider(Point size, Point cellSize)
            : base(size, cellSize, Vector2.Zero)
        {
        }

        public SimpleMapCollider(Point size, Point cellSize, Vector2 offset)
            : base(size, cellSize, offset)
        {
        }

        public SimpleMapCollider(int columns, int rows, int cellWidth, int cellHeight)
            : base(columns, rows, cellWidth, cellHeight, 0f, 0f)
        {
        }

        public SimpleMapCollider(int columns, int rows, int cellWidth, int cellHeight, float offsetX, float offsetY)
            : base(columns, rows, cellWidth, cellHeight, offsetX, offsetY)
        {
        }

        public override Collider Clone()
        {
            var collider = new SimpleMapCollider(MapSize, CellSize, Position);
            collider.Populate(Map.ToArray());
            return collider;
        }

        public override Shape ShapeAt(Point point)
        {
            return !Map.IsEmpty(point)
                ? new RectangleF(
                    AbsolutePosition + (point * CellSize).ToVector2(),
                    CellSize.ToVector2())
                : null;
        }
    }
}
