using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class Map<T>
    {
        private T[,] _region;

        public T this[int x, int y]
        {
            get { return _region.WithinRange(x, y) ? _region[x, y] : Empty; }
            set
            {
                if (!_region.WithinRange(x, y)) return;
                if (value == null && !value.Equals(Empty)) value = Empty;
                _region[x, y] = value;
            }
        }

        public Point Size { get; private set; }

        public int Columns => Size.X;

        public int Rows => Size.Y;

        public int Area => Size.X * Size.Y;

        public T Empty { get; private set; }

        public Map(Point size, T empty = default(T))
        {
            Size = size.Abs();
            Empty = empty;
            _region = new T[Columns, Rows];

            for (int i = 0; i < Area; i++)
            {
                var x = i % Columns;
                var y = i / Columns;
                _region[x, y] = Empty;
            }
        }

        public Map(int columns, int rows, T empty = default(T))
            : this(new Point(columns, rows), empty)
        {
        }

        public Map(T[,] map, T empty = default(T))
            : this(map.GetLength(0), map.GetLength(1), empty)
        {
            var columns = Math.Min(Columns, map.GetLength(0));
            var rows = Math.Min(Rows, map.GetLength(1));

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;
                _region[x, y] = map[x, y];
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Columns * Rows; i++)
            {
                var x = i % Columns;
                var y = i / Columns;
                _region[x, y] = Empty;
            }
        }

        public void Resize(Point size)
        {
            Resize(Point.Zero, size.Abs());
        }

        public void Resize(int columns, int rows)
        {
            Resize(Point.Zero, new Point(columns, rows).Abs());
        }

        public void Resize(Point from, Point to)
        {
            var size = (to - from).Abs();
            var region = new T[size.X, size.Y];

            for (int i = 0; i < size.X * size.Y; i++)
            {
                var x = i % size.X;
                var y = i / size.X;

                region[x, y] = _region.WithinRange(from.X + x, from.Y + y)
                             ? _region[from.X + x, from.Y + y] : Empty;
            }

            _region = region;
            Size = size;
        }

        public void Resize(int x1, int y1, int x2, int y2)
        {
            Resize(new Point(x1, y1), new Point(x2, y2));
        }

        public bool IsEmpty(Point cell) => _region[cell.X, cell.Y].Equals(Empty);

        public bool IsEmpty(int x, int y) => IsEmpty(new Point(x, y));

        public Map<T> Clone()
        {
            return new Map<T>(_region, Empty);
        }

        public T[,] ToArray()
        {
            var region = new T[Columns, Rows];

            for (int i = 0; i < Area; i++)
            {
                var x = i % Columns;
                var y = i / Columns;
                region[x, y] = _region[x, y];
            }

            return region;
        }
    }
}
