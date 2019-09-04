namespace FrogWorks
{
    public class SimpleMapCollider : MapCollider<bool>
    {
        public SimpleMapCollider(int columns, int rows, int cellWidth, int cellHeight, float x = 0, float y = 0)
            : base(columns, rows, cellWidth, cellHeight, x, y) { }

        public override Collider Clone()
        {
            var collider = new SimpleMapCollider(Columns, Rows, CellWidth, CellHeight, X, Y);
            collider.Populate(Map.ToArray());
            return collider;
        }
    }
}
