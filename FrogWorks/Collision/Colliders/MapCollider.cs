using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class MapCollider<T> : Collider
    {
        private Point _cellSize;

        protected Map<T> Map { get; private set; }

        public Point MapSize => new Point(Map.Columns, Map.Rows);

        public int Columns => MapSize.X;

        public int Rows => MapSize.Y;

        public Point CellSize
        {
            get { return _cellSize; }
            set
            {
                value = value.Abs();
                if (_cellSize == value) return;
                _cellSize = value;
                OnInternalTransformed();
            }
        }

        public int CellWidth
        {
            get { return CellSize.X; }
            set { CellSize = new Point(value, CellSize.Y); }
        }

        public int CellHeight
        {
            get { return CellSize.Y; }
            set { CellSize = new Point(CellSize.X, value); }
        }

        public override Vector2 Size
        {
            get { return (MapSize * CellSize).ToVector2(); }
            set
            {
                var mapSize = value.Abs().Divide(CellSize.ToVector2()).Round().ToPoint();
                if (MapSize == mapSize) return;
                Map.Resize(mapSize.X, mapSize.Y);
                OnInternalTransformed();
            }
        }

        public override Vector2 Upper
        {
            get { return AbsolutePosition; }
            set { AbsolutePosition = value; }
        }

        public override Vector2 Lower
        {
            get { return AbsolutePosition + Size; }
            set { AbsolutePosition = value - Size; }
        }

        protected MapCollider(int columns, int rows, int cellWidth, int cellHeight, float x = 0f, float y = 0f) 
            : base(new Vector2(x, y))
        {
            _cellSize = new Point(cellWidth, cellHeight).Abs();
            Map = new Map<T>(Math.Abs(columns), Math.Abs(rows));
        }

        public void Populate(T[,] data, int offsetX = 0, int offsetY = 0)
        {
            var columns = data.GetLength(0);
            var rows = data.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;

                Map[x + offsetX, y + offsetY] = data[x, y];
            }
        }

        public void Overlay(T[,] data, int offsetX = 0, int offsetY = 0)
        {
            var columns = data.GetLength(0);
            var rows = data.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;

                if (!data[x, y].Equals(Map.Empty))
                    Map[x + offsetX, y + offsetY] = data[x, y];
            }
        }

        public void Fill(T data, int x, int y, int columns, int rows)
        {
            var x1 = Math.Max(x, 0);
            var y1 = Math.Max(y, 0);
            var x2 = Math.Min(x + columns, Columns);
            var y2 = Math.Min(y + rows, Rows);

            var cellColumns = x2 - x1;
            var cellRows = y2 - y1;

            for (int i = 0; i < cellColumns * cellRows; i++)
            {
                var tx = i % cellColumns;
                var ty = i / cellColumns;

                Map[tx, ty] = data;
            }
        }

        public void Clear() => Map.Clear();

        protected IEnumerable<Point> GetCells(Vector2 point)
            => Extensions.AsEnumerable(point.SnapToGrid(CellSize.ToVector2(), AbsolutePosition).ToPoint());

        protected IEnumerable<Point> GetCells(Vector2 from, Vector2 to)
        {
            var cellFrom = from.SnapToGrid(CellSize.ToVector2(), AbsolutePosition).ToPoint();
            var cellTo = to.SnapToGrid(CellSize.ToVector2(), AbsolutePosition).ToPoint();
            var edge = cellTo - cellFrom;

            var isSteep = Math.Abs(edge.Y) > Math.Abs(edge.X);

            if (isSteep)
            {
                cellFrom = new Point(cellFrom.Y, cellFrom.X);
                cellTo = new Point(cellTo.Y, cellTo.X);
            }

            if (cellFrom.X > cellTo.X)
            {
                var temp = cellTo;
                cellFrom = cellTo;
                cellTo = temp;
            }

            var deltaX = edge.X;
            var deltaY = Math.Abs(edge.Y);
            var stepY = cellFrom.Y < cellTo.Y ? 1 : -1;
            var error = 0;
            var y = cellFrom.Y;

            for (int x = cellFrom.X; x < cellTo.X; x++)
            {
                yield return isSteep 
                    ? new Point(y, x) 
                    : new Point(x, y);

                error += deltaY;

                if (deltaX <= error * 2)
                {
                    y += stepY;
                    error -= deltaX;
                }
            }
        }

        protected IEnumerable<Point> GetCells(Rectangle area)
        {
            var bounds = area.SnapToGrid(CellSize.ToVector2(), AbsolutePosition);

            for (int i = 0; i < bounds.Width * bounds.Height; i++)
            {
                yield return new Point(
                    bounds.Left + (i % bounds.Width),
                    bounds.Top + (i / bounds.Width));
            }
        }

        protected virtual Shape GetCellShape(Point cell)
        {
            if (!Map.IsEmpty(cell.X, cell.Y))
            {
                return new RectangleF(
                    AbsolutePosition + (cell * CellSize).ToVector2(),
                    CellSize.ToVector2());
            }

            return null;
        }

        protected bool CheckCells(IEnumerable<Point> cells, Func<Shape, bool> predicate)
        {
            if (IsCollidable)
            {
                foreach (var cell in cells)
                {
                    var shape = GetCellShape(cell);
                    if (shape != null && predicate(shape))
                        return true;
                }
            }

            return false;
        }
    }
}
