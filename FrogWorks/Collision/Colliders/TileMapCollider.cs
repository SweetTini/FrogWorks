﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class TileMapCollider : Collider
    {
        private Point _tileSize;

        protected Dictionary<int, CollidableTile> Tiles { get; private set; }

        protected Map<int> Map { get; private set; }

        protected Rectangle DrawableRegion { get; private set; }

        public Point Area => new Point(Map.Columns, Map.Rows);

        public int Columns => Map.Columns;

        public int Rows => Map.Rows;

        public Point TileSize
        {
            get { return _tileSize; }
            set
            {
                value = value.Abs();
                if (value == _tileSize) return;
                _tileSize = value;
                OnTransformed();
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

        public override Vector2 Size
        {
            get { return (Area * TileSize).ToVector2(); }

            set
            {
                var newArea = value.Abs().SnapToGrid(TileSize.ToVector2()).ToPoint();
                if (newArea == Area) return;
                Map.Resize(newArea.X, newArea.Y);
                OnTransformed();
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

        public TileMapCollider(int columns, int rows, int tileWidth, int tileHeight, float offsetX = 0f, float offsetY = 0f)
            : base()
        {
            Tiles = new Dictionary<int, CollidableTile>();
            Map = new Map<int>(Math.Abs(columns), Math.Abs(rows));
            TileWidth = Math.Abs(tileWidth);
            TileHeight = Math.Abs(tileHeight);
            Position = new Vector2(offsetX, offsetY);
        }

        public override void Draw(RendererBatch batch, Color color)
        {
            for (int i = 0; i < DrawableRegion.Width * DrawableRegion.Height; i++)
            {
                var x = DrawableRegion.Left + (i % DrawableRegion.Width);
                var y = DrawableRegion.Top + (i / DrawableRegion.Width);
                GetTileShape(x, y)?.Shape.Draw(batch, color);
            }
        }

        public override bool Contains(Vector2 point)
        {
            return IsCollidable && (GetCollidedRegion(point)?.Shape.Contains(point) ?? false);
        }

        public override bool CastRay(Ray ray, out Raycast hit)
        {
            hit = new Raycast(ray);

            if (IsCollidable)
                foreach (var tile in GetCollidedRegion(ray))
                    if (ray.Cast(tile.Shape, out hit))
                        return true;

            return false;
        }

        public override bool Collide(Shape other)
        {
            if (IsCollidable)
                foreach (var tile in GetCollidedRegion(other))
                    if (tile.Shape.Collide(other))
                        return true;

            return false;
        }

        public override bool Collide(Shape other, out Manifold hit)
        {
            hit = new Manifold();

            if (IsCollidable)
                foreach (var tile in GetCollidedRegion(other))
                    if (tile.Shape.Collide(other, out hit))
                        return true;

            return false;
        }

        public override bool Collide(Collider other)
        {
            if (!Equals(other) && IsCollidable && other.IsCollidable)
                if (other is ShapeCollider<Shape>)
                    return Collide((other as ShapeCollider<Shape>).Shape);

            return false;
        }

        public override bool Collide(Collider other, out Manifold hit)
        {
            hit = new Manifold();

            if (!Equals(other) && IsCollidable && other.IsCollidable)
                if (other is ShapeCollider<Shape>)
                    return Collide((other as ShapeCollider<Shape>).Shape, out hit);

            return false;
        }

        public override Collider Clone()
        {
            return new TileMapCollider(Columns, Rows, TileWidth, TileHeight, X, Y)
            {
                Tiles = new Dictionary<int, CollidableTile>(Tiles),
                Map = new Map<int>(Map.ToArray(), Map.Empty)
            };
        }

        internal override void OnEntityAdded(Entity entity)
        {
            Parent.Parent.Camera.OnCameraUpdated += UpdateDrawableRegion;
            UpdateDrawableRegion(Parent.Parent.Camera);
        }

        internal override void OnEntityRemoved(Entity entity)
        {
            Parent.Parent.Camera.OnCameraUpdated -= UpdateDrawableRegion;
        }

        internal override void OnLayerChanged(Layer layer, Layer lastLayer)
        {
            lastLayer.Camera.OnCameraUpdated -= UpdateDrawableRegion;
            layer.Camera.OnCameraUpdated += UpdateDrawableRegion;
            UpdateDrawableRegion(layer.Camera);
        }

        internal override void OnTransformed()
        {
            var camera = Parent.Parent?.Camera;
            if (camera != null) UpdateDrawableRegion(camera);
        }

        #region Define Tiles
        public void AddOrUpdate(int index, Shape shape, CollisionType type = CollisionType.Solid)
        {
            if (Tiles.ContainsKey(index))
                Tiles[index] = CreateTile(shape, type);
            else Tiles.Add(index, CreateTile(shape, type));
        }

        public void Remove(int index)
        {
            if (Tiles.ContainsKey(index))
                Tiles.Remove(index);
        }

        public void RemoveAll()
        {
            Tiles.Clear();
        }
        #endregion

        #region Define Map
        public void PopulateMap(int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            var tileColumns = tiles.GetLength(0);
            var tileRows = tiles.GetLength(1);

            for (int i = 0; i < tileColumns * tileRows; i++)
            {
                var x = i % tileColumns;
                var y = i / tileColumns;
                Map[x + offsetX, y + offsetY] = tiles[x, y];
            }
        }

        public void OverlayMap(int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            var tileColumns = tiles.GetLength(0);
            var tileRows = tiles.GetLength(1);

            for (int i = 0; i < tileColumns * tileRows; i++)
            {
                var x = i % tileColumns;
                var y = i / tileColumns;
                var index = tiles[x, y];

                if (index != Map.Empty)
                    Map[x + offsetX, y + offsetY] = index;
            }
        }

        public void FillMap(int index, int x, int y, int columns, int rows)
        {
            var x1 = Math.Max(x, 0);
            var y1 = Math.Max(y, 0);
            var x2 = Math.Min(x + columns, Columns);
            var y2 = Math.Min(y + rows, Rows);

            var tileColumns = x2 - x1;
            var tileRows = y2 - y1;

            for (int i = 0; i < tileColumns * tileRows; i++)
            {
                var tx = i % tileColumns;
                var ty = i / tileColumns;
                Map[tx, ty] = index;
            }
        }

        public void ClearMap()
        {
            Map.Clear();
        }
        #endregion

        #region Helper Methods
        public CollidableTileShape GetCollidedRegion(Vector2 point)
        {
            var cell = point.SnapToGrid(TileSize.ToVector2(), AbsolutePosition).ToPoint();
            return GetTileShape(cell.X, cell.Y);
        }

        public IEnumerable<CollidableTileShape> GetCollidedRegion(Vector2 lineFrom, Vector2 lineTo)
        {
            var start = lineFrom.SnapToGrid(TileSize.ToVector2(), AbsolutePosition).ToPoint();
            var end = lineTo.SnapToGrid(TileSize.ToVector2(), AbsolutePosition).ToPoint();
            var edge = end - start;
            var isSteep = Math.Abs(edge.Y) > Math.Abs(edge.X);

            if (isSteep)
            {
                start = new Point(start.Y, start.X);
                end = new Point(end.Y, end.X);
            }

            if (start.X > end.X)
            {
                var temp = end;
                start = end;
                end = temp;
            }

            var deltaX = edge.X;
            var deltaY = Math.Abs(edge.Y);
            var stepY = start.Y < end.Y ? 1 : -1;
            var y = start.Y;
            var error = 0;

            for (int x = start.X; x <= end.X; x++)
            {
                yield return isSteep ? GetTileShape(y, x) : GetTileShape(x, y);
                error += deltaY;

                if (error * 2 >= deltaX)
                {
                    y += stepY;
                    error -= deltaX;
                }
            }
        }

        public IEnumerable<CollidableTileShape> GetCollidedRegion(Ray ray)
        {
            return GetCollidedRegion(ray.Position, ray.Endpoint);
        }

        public IEnumerable<CollidableTileShape> GetCollidedRegion(Rectangle bounds)
        {
            var gridBounds = bounds.SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

            for (int i = 0; i < gridBounds.Width * gridBounds.Height; i++)
            {
                var x = gridBounds.Left + (i % gridBounds.Width);
                var y = gridBounds.Top + (i / gridBounds.Width);
                var tileShape = GetTileShape(x, y);

                if (tileShape != null)
                    yield return tileShape;
            }
        }

        public IEnumerable<CollidableTileShape> GetCollidedRegion(Shape shape)
        {
            return GetCollidedRegion(shape.Bounds);
        }

        protected CollidableTile CreateTile(Shape shape, CollisionType collisionType)
        {
            var info = default(CollidableTile);

            if (shape is Circle)
            {
                info = new CollidableTile(null, TileShapeType.Circle, collisionType);
            }
            else if (shape is RectangleF)
            {
                var rectangle = shape as RectangleF;
                var scale = Math.Max(rectangle.Width, rectangle.Height).Inverse() * Vector2.One;
                var vertices = rectangle.ToVertices().Transform(scale: scale);
                info = new CollidableTile(vertices, TileShapeType.Rectangle, collisionType);
            }
            else if (shape is Polygon)
            {
                var polygon = shape as Polygon;
                var scale = Math.Max(polygon.Width, polygon.Height).Inverse() * Vector2.One;
                var vertices = polygon.Vertices.Transform(scale: scale);
                info = new CollidableTile(vertices, TileShapeType.Polygon, collisionType);
            }

            return info;
        }

        protected CollidableTileShape GetTileShape(int x, int y)
        {
            CollidableTile info;

            if (Tiles.TryGetValue(Map[x, y], out info))
            {
                var tileSize = TileSize.ToVector2();
                var position = new Vector2(x, y) * tileSize + AbsolutePosition;

                if (info.ShapeType == TileShapeType.Circle)
                {
                    var radius = Math.Min(tileSize.X, tileSize.Y) / 2f;
                    var circle = new Circle(position + Vector2.One * radius, radius);
                    return new CollidableTileShape(circle, info.CollisionType);
                }
                else if (info.ShapeType == TileShapeType.Rectangle)
                {
                    var vertices = info.Vertices.Transform(position, scale: tileSize);
                    var rectangle = new RectangleF(vertices[0], vertices[2] - vertices[0]);
                    return new CollidableTileShape(rectangle, info.CollisionType);
                }
                else if (info.ShapeType == TileShapeType.Polygon)
                {
                    var vertices = info.Vertices.Transform(position, scale: tileSize);
                    var polygon = new Polygon(vertices);
                    return new CollidableTileShape(polygon, info.CollisionType);
                }
            }

            return null;
        }

        private void UpdateDrawableRegion(Camera camera)
        {
            var x1 = (int)Math.Max(Math.Floor((camera.Bounds.Left - AbsoluteX) / TileWidth), 0);
            var y1 = (int)Math.Max(Math.Floor((camera.Bounds.Top - AbsoluteY) / TileHeight), 0);
            var x2 = (int)Math.Min(Math.Ceiling((camera.Bounds.Right + AbsoluteX) / TileWidth), Columns);
            var y2 = (int)Math.Min(Math.Ceiling((camera.Bounds.Bottom + AbsoluteY) / TileHeight), Rows);

            DrawableRegion = new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
        #endregion
    }

    public class CollidableTile
    {
        public Vector2[] Vertices { get; private set; }

        public TileShapeType ShapeType { get; private set; }

        public CollisionType CollisionType { get; private set; }

        internal CollidableTile(Vector2[] vertices, TileShapeType shapeType, CollisionType collisionType)
            : base()
        {
            ShapeType = shapeType;
            CollisionType = collisionType;
            Vertices = vertices;
        }
    }

    public class CollidableTileShape
    {
        public Shape Shape { get; private set; }

        public CollisionType CollisionType { get; private set; }

        internal CollidableTileShape(Shape shape, CollisionType collisionType)
            : base()
        {
            Shape = shape;
            CollisionType = collisionType;
        }
    }

    public enum TileShapeType
    {
        None,
        Circle,
        Rectangle,
        Polygon
    }

    public enum CollisionType
    {
        None,
        Solid,
        JumpThrough
    }
}
