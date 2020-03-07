using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class TileMapCollider : Collider
    {
        Camera _camera;
        Rectangle _drawRegion;
        Point _tileSize;

        protected Map<int> Map { get; private set; }

        public Point MapSize
        {
            get { return Map.Size; }
            set
            {
                value = value.Abs();

                if (Map.Size != value)
                {
                    Map.Resize(value);
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
                var gridPosition = point.SnapToGrid(
                    _tileSize.ToVector2(),
                    AbsolutePosition);

                var shape = GetTileShape(gridPosition.ToPoint());
                return shape?.Contains(point) ?? false;
            }

            return false;
        }

        public sealed override bool Raycast(Vector2 start, Vector2 end, out Raycast hit)
        {
            if (base.Raycast(start, end, out hit))
            {
                var tileSize = TileSize.ToVector2();
                var gridStart = start.SnapToGrid(tileSize, AbsolutePosition);
                var gridEnd = end.SnapToGrid(tileSize, AbsolutePosition);

                foreach (var position in PlotLine(gridStart, gridEnd))
                {
                    var shape = GetTileShape(position.ToPoint());
                    if (shape?.Raycast(start, end, out hit) ?? false)
                        return true;
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

        IEnumerable<Vector2> PlotLine(Vector2 start, Vector2 end)
        {
            var steep = (end.Y - start.Y).Abs() > (end.X - start.X).Abs();

            if (steep)
            {
                Extensions.Swap(ref start.X, ref start.Y);
                Extensions.Swap(ref end.X, ref end.Y);
            }

            if (start.X > end.X)
            {
                Extensions.Swap(ref start.X, ref end.X);
                Extensions.Swap(ref start.Y, ref end.Y);
            }

            var dx = end.X - start.X;
            var dy = (end.Y - start.Y).Abs();
            var error = (dx * .5f).Floor();
            var yStep = start.Y < end.Y ? 1f : -1f;
            var y = start.Y;

            for (float x = start.X; x <= end.X; x++)
            {
                yield return steep
                    ? new Vector2(y, x)
                    : new Vector2(x, y);

                error -= dy;

                if (error < 0f)
                {
                    y += yStep;
                    error += dx;
                }
            }
        }
    }
}
