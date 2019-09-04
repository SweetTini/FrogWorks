namespace FrogWorks
{
    public class IndexMapCollider : MapCollider<int>
    {
        public IndexMapCollider(int columns, int rows, int cellWidth, int cellHeight, float x = 0, float y = 0)
            : base(columns, rows, cellWidth, cellHeight, x, y) { }

        public override Collider Clone()
        {
            var collider = new IndexMapCollider(Columns, Rows, CellWidth, CellHeight, X, Y);
            collider.Populate(Map.ToArray());
            return collider;
        }
    }
}
