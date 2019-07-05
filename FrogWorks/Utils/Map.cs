using System;

namespace FrogWorks
{
    public class Map<T>
    {
        private T[,] _area;

        public T this[int x, int y]
        {
            get { return _area.WithinRange(x, y) ? _area[x, y] : Empty; }
            set
            {
                if (!_area.WithinRange(x, y)) return;
                if (value == null && !value.Equals(Empty)) value = Empty;
                _area[x, y] = value;
            }
        }

        public int Columns { get; private set; }

        public int Rows { get; private set; }

        public T Empty { get; private set; }

        public Map(int columns, int rows, T empty = default(T))
        {
            Columns = columns;
            Rows = rows;
            Empty = empty;
            _area = new T[Columns, Rows];

            for (int i = 0; i < Columns * Rows; i++)
            {
                var x = i % Columns;
                var y = i / Columns;
                _area[x, y] = Empty;
            }
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
                _area[x, y] = map[x, y];
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Columns * Rows; i++)
            {
                var x = i % Columns;
                var y = i / Columns;
                _area[x, y] = Empty;
            }
        }

        public void Resize(int columns, int rows)
        {
            Resize(0, 0, columns, rows);
        }

        public void Resize(int x1, int y1, int x2, int y2)
        {
            var columns = Math.Abs(x2 - x1);
            var rows = Math.Abs(y2 - y1);
            var area = new T[columns, rows];

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;
                area[x, y] = _area.WithinRange(x + x1, y + y1) ? _area[x + x1, y + y1] : Empty;
            }

            _area = area;
            Columns = columns;
            Rows = rows;
        }

        public Map<T> Clone()
        {
            return new Map<T>(_area, Empty);
        }

        public T[,] ToArray()
        {
            var area = new T[Columns, Rows];

            for (int i = 0; i < Columns * Rows; i++)
            {
                var x = i % Columns;
                var y = i / Columns;
                area[x, y] = _area[x, y];
            }

            return area;
        }
    }
}
