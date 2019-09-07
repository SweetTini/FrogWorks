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

        protected MapCollider(int columns, int rows, int cellWidth, int cellHeight, float x = 0f, float y = 0f) 
            : base(new Vector2(x, y))
        {
            Map = new Map<T>(Math.Abs(columns), Math.Abs(rows));

            _cellSize = new Point(cellWidth, cellHeight).Abs();
        }

        public sealed override void Draw(RendererBatch batch, Color color, bool fill = false)
        {
            for (int i = 0; i < _drawableRegion.Width * _drawableRegion.Height; i++)
            {
                var x = _drawableRegion.Left + (i % _drawableRegion.Width);
                var y = _drawableRegion.Top + (i / _drawableRegion.Width);

                ShapeOf(x, y)?.Draw(batch, color, fill);
            }
        }

        public override bool Collide(Vector2 point)
            => Validate(At(point), (p, i) => CheckShape(p, s => s.Contains(point)));

        public override bool Collide(Ray ray)
            => Validate(At(ray.Position, ray.Endpoint), (p, i) => CheckShape(p, s => ray.Cast(s)));

        public override bool Collide(Shape shape)
            => Validate(At(shape.Bounds), (p, i) => CheckShape(p, s => shape.Collide(s)));

        public override bool Collide(Collider collider)
        {
            if (collider == null || Equals(collider) || !collider.IsCollidable) return false;

            return Validate(At(collider.Bounds), (p, i) 
                => CheckShape(p, s => collider is ShapeCollider && (collider as ShapeCollider).Collide(s)));
        }

        public void Populate(T[,] items, int offsetX = 0, int offsetY = 0)
        {
            var columns = items.GetLength(0);
            var rows = items.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;

                Map[x + offsetX, y + offsetY] = items[x, y];
            }
        }

        public void Overlay(T[,] items, int offsetX = 0, int offsetY = 0)
        {
            var columns = items.GetLength(0);
            var rows = items.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;
                if (items[x, y].Equals(Map.Empty)) continue;

                Map[x + offsetX, y + offsetY] = items[x, y];
            }
        }

        public void Fill(T item, int x, int y, int columns, int rows)
        {
            var x1 = Math.Max(x, 0);
            var y1 = Math.Max(y, 0);
            var x2 = Math.Min(x + columns, Columns);
            var y2 = Math.Min(y + rows, Rows);

            var cellColumns = x2 - x1;
            var cellRows = y2 - y1;

            for (int i = 0; i < cellColumns * cellRows; i++)
            {
                var tx = x1 + (i % cellColumns);
                var ty = y1 + (i / cellColumns);

                Map[tx, ty] = item;
            }
        }

        public void Clear() => Map.Clear();

        protected sealed override void OnAdded() => AddCameraUpdateEvent();

        protected sealed override void OnRemoved() => RemoveCameraUpdateEvent();

        protected sealed override void OnEntityAdded() => AddCameraUpdateEvent();

        protected sealed override void OnEntityRemoved() => RemoveCameraUpdateEvent();

        protected sealed override void OnLayerAdded() => AddCameraUpdateEvent();

        protected sealed override void OnLayerRemoved() => RemoveCameraUpdateEvent();

        protected sealed override void OnTransformed() => UpdateDrawableRegion(Layer?.Camera);

        protected IEnumerable<Point> At(Vector2 point) 
            => Extensions.AsEnumerable(point.SnapToGrid(CellSize.ToVector2(), AbsolutePosition).ToPoint());

        protected IEnumerable<Point> At(Vector2 from, Vector2 to)
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

        protected IEnumerable<Point> At(Rectangle area)
        {
            var bounds = area.SnapToGrid(CellSize.ToVector2(), AbsolutePosition);

            for (int i = 0; i < bounds.Width * bounds.Height; i++)
            {
                yield return new Point(
                    bounds.Left + (i % bounds.Width),
                    bounds.Top + (i / bounds.Width));
            }
        }

        protected abstract Shape ShapeOf(Point point);

        protected Shape ShapeOf(int x, int y) => ShapeOf(new Point(x, y));

        protected bool Validate(IEnumerable<Point> points, Func<Point, T, bool> predicate)
        {
            if (IsCollidable)
            {
                foreach (var point in points)
                {
                    var item = Map[point.X, point.Y];
                    if (item.Equals(Map.Empty)) continue;
                    if (predicate(point, item)) return true;
                }
            }

            return false;
        }

        protected bool CheckShape(Point point, Func<Shape, bool> predicate)
        {
            var shape = ShapeOf(point);

            return shape != null && predicate(shape);
        }

        private void AddCameraUpdateEvent()
        {
            if (!_isRegistered && Layer != null)
            {
                Layer.Camera.OnCameraUpdated += UpdateDrawableRegion;
                UpdateDrawableRegion(Layer.Camera);
                _isRegistered = true;
            }
        }

        private void RemoveCameraUpdateEvent()
        {
            if (_isRegistered && Layer != null)
            {
                Layer.Camera.OnCameraUpdated -= UpdateDrawableRegion;
                _isRegistered = false;
            }
        }

        private void UpdateDrawableRegion(Camera camera)
        {
            if (camera == null) return;

            var x1 = (int)Math.Max(Math.Floor((camera.Bounds.Left - AbsoluteX) / CellWidth), 0);
            var y1 = (int)Math.Max(Math.Floor((camera.Bounds.Top - AbsoluteY) / CellHeight), 0);
            var x2 = (int)Math.Min(Math.Ceiling((camera.Bounds.Right + AbsoluteX) / CellWidth), Columns);
            var y2 = (int)Math.Min(Math.Ceiling((camera.Bounds.Bottom + AbsoluteY) / CellHeight), Rows);

            _drawableRegion = new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
