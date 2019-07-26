using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public class TileMapCollider : Collider
    {
        private int _tileWidth, _tileHeight;

        protected Dictionary<int, TileCollisionInfo> CollisionInfos { get; private set; }

        protected Map<int> CollisionMap { get; private set; }

        public int Columns => CollisionMap.Columns;

        public int Rows => CollisionMap.Rows;

        public int TileWidth
        {
            get { return _tileWidth; }
            set
            {
                value = Math.Abs(value);
                if (value == _tileWidth) return;
                _tileWidth = value;
                OnTransformed();
            }
        }

        public int TileHeight
        {
            get { return _tileHeight; }
            set
            {
                value = Math.Abs(value);
                if (value == _tileHeight) return;
                _tileHeight = value;
                OnTransformed();
            }
        }

        public override Vector2 Size
        {
            get
            {
                var tileSize = new Vector2(_tileWidth, _tileHeight);
                var mapSize = new Vector2(CollisionMap.Columns, CollisionMap.Rows);
                return mapSize * tileSize;
            }

            set
            {
                var tileSize = new Vector2(_tileWidth, _tileHeight);
                var mapSize = new Vector2(CollisionMap.Columns, CollisionMap.Rows).ToPoint();
                var newMapSize = value.Abs().Divide(tileSize).Round().ToPoint();

                if (newMapSize == mapSize) return;
                CollisionMap.Resize(newMapSize.X, newMapSize.Y);
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
            CollisionInfos = new Dictionary<int, TileCollisionInfo>();
            CollisionMap = new Map<int>(Math.Abs(columns), Math.Abs(rows));
            TileWidth = Math.Abs(tileWidth);
            TileHeight = Math.Abs(tileHeight);
            Position = new Vector2(offsetX, offsetY);
        }

        public override bool Contains(Vector2 point)
        {
            throw new NotImplementedException();
        }

        public override bool CastRay(Ray ray, out Raycast hit)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Shape other)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Shape other, out Manifold hit)
        {
            throw new NotImplementedException();
        }

        public override Collider Clone()
        {
            throw new NotImplementedException();
        }

        #region Define Collisions
        public void AddOrUpdateInfo(int index, Shape shape, CollisionType type = CollisionType.Solid)
        {
            if (CollisionInfos.ContainsKey(index))
                CollisionInfos[index] = CreateCollisionInfo(index, shape, type);
            else CollisionInfos.Add(index, CreateCollisionInfo(index, shape, type));
        }

        public void RemoveInfo(int index)
        {
            if (CollisionInfos.ContainsKey(index))
                CollisionInfos.Remove(index);
        }

        public void ClearInfos()
        {
            CollisionInfos.Clear();
        }
        #endregion

        #region Define Map
        public void Populate(int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            var tileColumns = tiles.GetLength(0);
            var tileRows = tiles.GetLength(1);

            for (int i = 0; i < tileColumns * tileRows; i++)
            {
                var x = i % tileColumns;
                var y = i / tileColumns;
                CollisionMap[x + offsetX, y + offsetY] = tiles[x, y];
            }
        }

        public void Overlay(int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            var tileColumns = tiles.GetLength(0);
            var tileRows = tiles.GetLength(1);

            for (int i = 0; i < tileColumns * tileRows; i++)
            {
                var x = i % tileColumns;
                var y = i / tileColumns;
                var index = tiles[x, y];

                if (index != CollisionMap.Empty)
                    CollisionMap[x + offsetX, y + offsetY] = index;
            }
        }

        public void Fill(int index, int x, int y, int columns, int rows)
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
                CollisionMap[tx, ty] = index;
            }
        }

        public void Clear()
        {
            CollisionMap.Clear();
        }
        #endregion

        #region Helper Methods
        protected TileCollisionInfo CreateCollisionInfo(int index, Shape shape, CollisionType type)
        {
            var info = default(TileCollisionInfo);

            if (shape is RectangleF)
            {
                var rectangle = shape as RectangleF;
                var scale = Math.Max(rectangle.Width, rectangle.Height).Inverse() * Vector2.One;
                var vertices = rectangle.ToVertices().Transform(scale: scale);
                info = new TileCollisionInfo(index, vertices, ShapeType.Rectangle, type);
            }
            else if (shape is Circle)
            {
                var circle = shape as Circle;
                info = new TileCollisionInfo(index, new[] { circle.Center }, ShapeType.Circle, type);
            }
            else if (shape is Polygon)
            {
                var polygon = shape as Polygon;
                var scale = Math.Max(polygon.Width, polygon.Height).Inverse() * Vector2.One;
                var vertices = polygon.Vertices.Transform(scale: scale);
                info = new TileCollisionInfo(index, vertices, ShapeType.Polygon, type);
            }

            return info;
        }

        protected Shape GetCollidedArea(Vector2 point)
        {
            throw new NotImplementedException();
        }

        protected IEnumerable<Shape> GetCollidedArea(Vector2 lineFrom, Vector2 lineTo)
        {
            throw new NotImplementedException();
        }

        protected IEnumerable<Shape> GetCollidedArea(Rectangle bounds)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public struct TileCollisionInfo
    {
        public int Index { get; private set; }

        public Vector2[] Vertices { get; private set; }

        public ShapeType ShapeType { get; private set; }

        public CollisionType CollisionType { get; private set; }

        internal TileCollisionInfo(int index, Vector2[] vertices, ShapeType shapeType, CollisionType collisionType)
        {
            Index = index;
            ShapeType = shapeType;
            CollisionType = collisionType;
            Vertices = vertices;
        }
    }

    public enum ShapeType
    {
        None,
        Rectangle,
        Circle,
        Polygon
    }

    public enum CollisionType
    {
        None,
        Solid,
        JumpThrough
    }
}
