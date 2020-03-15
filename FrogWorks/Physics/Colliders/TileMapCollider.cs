using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class TileMapCollider : Collider
    {
        Camera _camera;
        Rectangle _drawRegion;
        Point _tileSize;

        protected internal Map<int> Map { get; internal set; }

        public Point MapSize
        {
            get { return Map.Size; }
            set
            {
                value = value.Abs();

                if (Map.Size != value)
                {
                    Map.Resize(value);
                    OnMapResized();
                    OnTransformedInternally();
                }
            }
        }

        public int MapWidth
        {
            get { return MapSize.X; }
            set { MapSize = new Point(value, MapSize.Y); }
        }

        public int MapHeight
        {
            get { return MapSize.Y; }
            set { MapSize = new Point(MapSize.X, value); }
        }

        public Point TileSize
        {
            get { return _tileSize; }
            set
            {
                value = value.Abs();

                if (_tileSize != value)
                {
                    _tileSize = value;
                    OnTransformedInternally();
                }
            }
        }

        public int TileWidth
        {
            get { return TileSize.X; }
            set { TileSize = new Point(value, TileSize.Y); }
        }

        public int TileHeight
        {
            get { return TileSize.Y; }
            set { TileSize = new Point(TileSize.X, value); }
        }

        public sealed override Vector2 Size
        {
            get { return (MapSize * TileSize).ToVector2(); }
            set { MapSize = value.ToPoint().Divide(TileSize); }
        }

        public sealed override Vector2 Min
        {
            get { return AbsolutePosition; }
            set { AbsolutePosition = value; }
        }

        public sealed override Vector2 Max
        {
            get { return AbsolutePosition + Size; }
            set { AbsolutePosition = value - Size; }
        }

        protected TileMapCollider(Vector2 position, Point mapSize, Point tileSize)
            : base(position)
        {
            Map = new Map<int>(mapSize);
            _tileSize = tileSize.Abs();
        }

        public sealed override bool Contains(Vector2 point)
        {
            if (base.Contains(point))
            {
                var location = point
                    .SnapToGrid(TileSize.ToVector2(), AbsolutePosition)
                    .ToPoint();

                var tile = GetTileShape(location);
                return tile?.Contains(point) ?? false;
            }

            return false;
        }

        public sealed override bool Raycast(Vector2 start, Vector2 end, out Raycast hit)
        {
            if (base.Raycast(start, end, out hit))
            {
                var tileSize = TileSize.ToVector2();
                var gStart = start.SnapToGrid(tileSize, AbsolutePosition);
                var gEnd = end.SnapToGrid(tileSize, AbsolutePosition);

                foreach (var location in PlotLine(gStart, gEnd))
                {
                    var tile = GetTileShape(location);
                    var hitDetected = tile?.Raycast(start, end, out hit) ?? false;
                    if (hitDetected) return true;
                }
            }

            return false;
        }

        public sealed override bool Overlaps(Shape shape)
        {
            if (base.Overlaps(shape))
            {
                var region = shape.Bounds
                    .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                foreach (var location in PlotRegion(region))
                {
                    var tile = GetTileShape(location);
                    var overlaps = tile?.Overlaps(shape) ?? false;
                    if (overlaps) return true;
                }
            }

            return false;
        }

        public sealed override bool Overlaps(Shape shape, out CollisionResult result)
        {
            var collide = false;

            if (base.Overlaps(shape, out result))
            {
                var region = shape.Bounds
                    .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                foreach (var location in PlotRegion(region))
                {
                    Manifold hit = default;
                    var tile = GetTileShape(location);
                    var overlaps = tile?.Overlaps(shape, out hit) ?? false;

                    if (overlaps)
                    {
                        hit.Normal = -hit.Normal;
                        result.Add(hit);
                        collide = true;
                    }
                }
            }

            return collide;
        }

        public sealed override bool Overlaps(Collider collider)
        {
            if (base.Overlaps(collider))
            {
                if (collider is ShapeCollider)
                {
                    var shape = (collider as ShapeCollider).Shape;
                    var region = shape.Bounds
                        .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                    foreach (var location in PlotRegion(region))
                    {
                        var tile = GetTileShape(location);
                        var overlaps = tile?.Overlaps(shape) ?? false;
                        if (overlaps) return true;
                    }
                }
            }

            return false;
        }

        public sealed override bool Overlaps(Collider collider, out CollisionResult result)
        {
            if (base.Overlaps(collider, out result))
            {
                if (collider is ShapeCollider)
                {
                    var collide = false;
                    var shape = (collider as ShapeCollider).Shape;
                    var region = shape.Bounds
                        .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                    foreach (var location in PlotRegion(region))
                    {
                        Manifold hit = default;
                        var tile = GetTileShape(location);
                        var overlaps = tile?.Overlaps(shape, out hit) ?? false;

                        if (overlaps)
                        {
                            hit.Normal = -hit.Normal;
                            result.Add(hit);
                            collide = true;
                        }
                    }

                    if (collide) return true;
                }
            }

            return false;
        }

        public sealed override void Draw(RendererBatch batch, Color color)
        {
            for (int i = 0; i < _drawRegion.Width * _drawRegion.Height; i++)
            {
                var x = _drawRegion.Left + (i % _drawRegion.Width);
                var y = _drawRegion.Top + (i / _drawRegion.Width);

                DrawTileShape(batch, color, new Point(x, y));
            }
        }

        public virtual void Reset(bool hardReset = false)
        {
            Map.Clear();
        }

        protected abstract Shape GetTileShape(Point position);

        protected virtual void DrawTileShape(
            RendererBatch batch,
            Color color,
            Point position)
        {
            var shape = GetTileShape(position);
            shape?.Draw(batch, color);
        }

        protected sealed override void OnAdded()
        {
            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnRemoved()
        {
            RemoveLinkToCamera();

            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnEntityAdded()
        {
            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnEntityRemoved()
        {
            RemoveLinkToCamera();

            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnLayerAdded()
        {
            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnLayerRemoved()
        {
            RemoveLinkToCamera();

            var camera = Layer?.Camera ?? Scene?.Camera;
            AddLinkToCamera(camera);
        }

        protected sealed override void OnTransformed()
        {
            UpdateDrawRegion(_camera);
        }

        protected virtual void OnMapResized()
        {
        }

        void AddLinkToCamera(Camera camera)
        {
            if (camera != null)
            {
                if (_camera == null)
                {
                    _camera = camera;
                    _camera.OnChanged += UpdateDrawRegion;
                    UpdateDrawRegion(_camera);
                }
                else if (_camera != camera)
                {
                    RemoveLinkToCamera();
                    AddLinkToCamera(camera);
                }
            }
        }

        void RemoveLinkToCamera()
        {
            if (_camera != null)
            {
                _camera.OnChanged -= UpdateDrawRegion;
                _camera = null;
            }
        }

        void UpdateDrawRegion(Camera camera)
        {
            var min = Point.Zero;
            var max = MapSize;

            if (_camera != null)
            {
                var cameraMin = camera.View.Location.ToVector2();
                var cameraMax = cameraMin + camera.View.Size.ToVector2();
                var tileSize = TileSize.ToVector2();

                min = (cameraMin - AbsolutePosition).Divide(tileSize).Floor().ToPoint();
                max = (cameraMax + AbsolutePosition).Divide(tileSize).Ceiling().ToPoint();
            }

            _drawRegion = new Rectangle(min, max - min);
        }

        protected IEnumerable<Point> PlotLine(Vector2 start, Vector2 end)
        {
            var p1 = start.ToPoint();
            var p2 = end.ToPoint();
            var steep = (p2.Y - p1.Y).Abs() > (p2.X - p1.X).Abs();

            if (steep)
            {
                Extensions.Swap(ref p1.X, ref p1.Y);
                Extensions.Swap(ref p2.X, ref p2.Y);
            }

            if (p1.X > p2.X)
            {
                Extensions.Swap(ref p1.X, ref p2.X);
                Extensions.Swap(ref p1.Y, ref p2.Y);
            }

            var dx = p2.X - p1.X;
            var dy = (p2.Y - p1.Y).Abs();
            var error = dx / 2;
            var yStep = p1.Y < p2.Y ? 1 : -1;
            var y = p1.Y;

            for (int x = p1.X; x <= p2.X; x++)
            {
                yield return steep
                    ? new Point(y, x)
                    : new Point(x, y);

                error -= dy;

                if (error < 0)
                {
                    y += yStep;
                    error += dx;
                }
            }
        }

        protected IEnumerable<Point> PlotRegion(Rectangle region)
        {
            for (int i = 0; i < region.Width * region.Height; i++)
            {
                var x = region.Left + (i % region.Width);
                var y = region.Top + (i / region.Width);

                yield return new Point(x, y);
            }
        }
    }

    public interface IMapModifier<T>
    {
        void Fill(T tile, int x, int y);

        void Fill(T tile, Point location);

        void Fill(T tile, int x, int y, int width, int height);

        void Fill(T tile, Point location, Point size);

        void Populate(T[,] tiles, int offsetX, int offsetY);

        void Populate(T[,] tiles, Point offset);

        void Overlay(T[,] tiles, int offsetX, int offsetY);

        void Overlay(T[,] tiles, Point offset);
    }

    public interface IMapAccessor<T>
    {
        T GetTile(float x, float y);

        T GetTile(Vector2 point);

        IEnumerable<T> GetTiles(float x1, float y1, float x2, float y2);

        IEnumerable<T> GetTiles(Vector2 start, Vector2 end);

        IEnumerable<T> GetTiles(Shape shape);

        IEnumerable<T> GetTiles(Collider collider);

        IEnumerable<T> GetTiles(Entity entity);
    }
}
