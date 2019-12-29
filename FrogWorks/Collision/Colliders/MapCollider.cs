using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class MapCollider<T> : Collider
    {
        private Point _cellSize;
        private Rectangle _drawableRegion;
        private bool _isRegistered;

        protected Map<T> Map { get; private set; }

        public Point MapSize => Map.Size;

        public int Columns => Map.Columns;

        public int Rows => Map.Rows;

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

        public sealed override Vector2 Size
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

        public sealed override Vector2 Upper
        {
            get { return AbsolutePosition; }
            set { AbsolutePosition = value; }
        }

        public sealed override Vector2 Lower
        {
            get { return AbsolutePosition + Size; }
            set { AbsolutePosition = value - Size; }
        }

        protected MapCollider(Point size, Point cellSize, Vector2 offset)
            : base(offset)
        {
            Map = new Map<T>(size);

            _cellSize = cellSize.Abs();
        }

        protected MapCollider(int columns, int rows, int cellWidth, int cellHeight, float offsetX, float offsetY)
            : this(new Point(columns, rows), new Point(cellWidth, cellHeight), new Vector2(offsetX, offsetY))
        {
        }

        public sealed override void Draw(RendererBatch batch, Color stroke, Color? fill = null)
        {
            for (int i = 0; i < _drawableRegion.Width * _drawableRegion.Height; i++)
            {
                var x = _drawableRegion.Left + (i % _drawableRegion.Width);
                var y = _drawableRegion.Top + (i / _drawableRegion.Width);

                DrawShapeAt(new Point(x, y), batch, stroke, fill);
            }
        }

        public sealed override bool Collide(Vector2 point)
            => Validate(Place(point), (p, e) => Validate(p, s => s.Contains(point)));

        public sealed override bool Collide(Ray ray)
            => Validate(Place(ray), (p, e) => Validate(p, s => ray.Cast(s)));

        public sealed override bool Collide(Shape shape)
            => Validate(Place(shape), (p, e) => Validate(p, s => shape.Collide(s)));

        public sealed override bool Collide(Collider collider)
        {
            var isValid = collider != null
                && !Equals(collider)
                && collider.IsCollidable
                && collider is ShapeCollider;

            return isValid && Validate(Place(collider), 
                (p, e) => Validate(p, s => (collider as ShapeCollider).Collide(s)));
        }

        public void Populate(T[,] items)
        {
            Populate(items, Point.Zero);
        }

        public void Populate(T[,] items, Point offset)
        {
            var columns = items.GetLength(0);
            var rows = items.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;

                Map[x + offset.X, y + offset.Y] = items[x, y];
            }
        }

        public void Populate(T[,] items, int offsetX, int offsetY)
        {
            Populate(items, new Point(offsetX, offsetY));
        }

        public void Overlay(T[,] items)
        {
            Overlay(items, Point.Zero);
        }

        public void Overlay(T[,] items, Point offset)
        {
            var columns = items.GetLength(0);
            var rows = items.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;
                if (items[x, y].Equals(Map.Empty)) continue;

                Map[x + offset.X, y + offset.Y] = items[x, y];
            }
        }

        public void Overlay(T[,] items, int offsetX, int offsetY)
        {
            Overlay(items, new Point(offsetX, offsetY));
        }

        public void Fill(T item, Point location)
        {
            Fill(item, location, new Point(1, 1));
        }

        public void Fill(T item, Point location, Point size)
        {
            var upper = location.Max(Point.Zero);
            var lower = (location + size).Min(MapSize);
            var cellSize = lower - upper;

            for (int i = 0; i < cellSize.X * cellSize.Y; i++)
            {
                var tx = upper.X + (i % cellSize.X);
                var ty = upper.Y + (i / cellSize.X);

                Map[tx, ty] = item;
            }
        }

        public void Fill(T item, int x, int y)
        {
            Fill(item, new Point(x, y), new Point(1, 1));
        }

        public void Fill(T item, int x, int y, int columns, int rows)
        {
            Fill(item, new Point(x, y), new Point(columns, rows));
        }

        public void Clear() => Map.Clear();

        public Point Place(Vector2 point) => point.SnapToGrid(CellSize.ToVector2(), AbsolutePosition).ToPoint();

        public IEnumerable<Point> Place(Vector2 from, Vector2 to)
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

        public IEnumerable<Point> Place(Rectangle area)
        {
            var bounds = area.SnapToGrid(CellSize.ToVector2(), AbsolutePosition);

            for (int i = 0; i < bounds.Width * bounds.Height; i++)
            {
                yield return new Point(
                    bounds.Left + (i % bounds.Width),
                    bounds.Top + (i / bounds.Width));
            }
        }

        public IEnumerable<Point> Place(Ray ray) => Place(ray.Position, ray.Endpoint);

        public IEnumerable<Point> Place(Shape shape) => Place(shape.Bounds);

        public IEnumerable<Point> Place(Collider collider) => Place(collider?.Bounds ?? Rectangle.Empty);

        public IEnumerable<Point> Place(Entity entity) => Place(entity?.Collider);

        public T ElementAt(Point point) => Map[point.X, point.Y];

        public T ElementAt(int x, int y) => ElementAt(new Point(x, y));

        public IEnumerable<T> ElementAt(IEnumerable<Point> points)
        {
            foreach (var point in points)
                yield return Map[point.X, point.Y];
        }

        public abstract Shape ShapeAt(Point point);

        public Shape ShapeAt(int x, int y) => ShapeAt(new Point(x, y));

        public bool IsEmpty(Point point) => Map[point.X, point.Y].Equals(Map.Empty);

        public bool IsEmpty(int x, int y) => IsEmpty(new Point(x, y));

        public bool IsEmpty(IEnumerable<Point> points)
        {
            foreach (var point in points)
                if (Map[point.X, point.Y].Equals(Map.Empty))
                    return true;

            return false;
        }

        public bool Validate(Point point, Func<Point, T, bool> predicate)
            => Validate(Extensions.Enumerate(point), predicate);

        public bool Validate(IEnumerable<Point> points, Func<Point, T, bool> predicate)
        {
            if (IsCollidable)
            {
                foreach (var point in points)
                {
                    var element = ElementAt(point);
                    if (element.Equals(Map.Empty)) continue;
                    if (predicate(point, element)) return true;
                }
            }

            return false;
        }

        protected bool Validate(Point point, Func<Shape, bool> predicate)
        {
            var shape = ShapeAt(point);

            return shape != null
                ? predicate(shape)
                : false;
        }

        protected virtual void DrawShapeAt(Point point, RendererBatch batch, Color stroke, Color? fill = null)
            => ShapeAt(point)?.Draw(batch, stroke, fill);

        protected sealed override void OnAdded() => AddCameraUpdateEvent();

        protected sealed override void OnRemoved() => RemoveCameraUpdateEvent();

        protected sealed override void OnEntityAdded() => AddCameraUpdateEvent();

        protected sealed override void OnEntityRemoved() => RemoveCameraUpdateEvent();

        protected sealed override void OnLayerAdded() => AddCameraUpdateEvent();

        protected sealed override void OnLayerRemoved() => RemoveCameraUpdateEvent();

        protected sealed override void OnTransformed() => UpdateDrawableRegion(Layer?.Camera);

        private void AddCameraUpdateEvent()
        {
            if (!_isRegistered && Layer != null)
            {
                Layer.Camera.OnChanged += UpdateDrawableRegion;
                UpdateDrawableRegion(Layer.Camera);
                _isRegistered = true;
            }
        }

        private void RemoveCameraUpdateEvent()
        {
            if (_isRegistered && Layer != null)
            {
                Layer.Camera.OnChanged -= UpdateDrawableRegion;
                _isRegistered = false;
            }
        }

        private void UpdateDrawableRegion(Camera camera)
        {
            if (camera == null) return;

            var x1 = (int)Math.Max(Math.Floor((camera.View.Left - AbsoluteX) / CellWidth), 0);
            var y1 = (int)Math.Max(Math.Floor((camera.View.Top - AbsoluteY) / CellHeight), 0);
            var x2 = (int)Math.Min(Math.Ceiling((camera.View.Right + AbsoluteX) / CellWidth), Columns);
            var y2 = (int)Math.Min(Math.Ceiling((camera.View.Bottom + AbsoluteY) / CellHeight), Rows);

            _drawableRegion = new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
