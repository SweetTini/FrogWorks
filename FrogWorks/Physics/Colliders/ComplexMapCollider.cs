using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public sealed class ComplexMapCollider<T> :
        TileMapCollider,
        ITaggedMapCollisionHandler<T>,
        IMapModifier<int>,
        IMapAccessor<ComplexTile<T>>
        where T : struct
    {
        Map<T> AttributeMap { get; set; }

        Dictionary<int, Tile> Tiles { get; set; }

        public ComplexMapCollider(
            int columns, int rows,
            int tileWidth, int tileHeight)
            : this(
                  Vector2.Zero,
                  new Point(columns, rows),
                  new Point(tileWidth, tileHeight))
        {
        }

        public ComplexMapCollider(
            float x, float y,
            int columns, int rows,
            int tileWidth, int tileHeight)
            : this(
                  new Vector2(x, y),
                  new Point(columns, rows),
                  new Point(tileWidth, tileHeight))
        {
        }

        public ComplexMapCollider(Point mapSize, Point tileSize)
            : this(Vector2.Zero, mapSize, tileSize)
        {
        }

        public ComplexMapCollider(
            Vector2 position,
            Point mapSize,
            Point tileSize)
            : base(position, mapSize, tileSize)
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("Generic type must be an Enum.");

            AttributeMap = new Map<T>(mapSize);
            Tiles = new Dictionary<int, Tile>();
        }

        protected override Shape GetTileShape(Point position)
        {
            var index = Map[position];

            if (index != Map.Empty)
            {
                Tile tile;

                if (Tiles.TryGetValue(index, out tile))
                {
                    var tileSize = TileSize.ToVector2();
                    var offset = AbsolutePosition + position.ToVector2() * tileSize;
                    var vertices = tile.Vertices.Transform(scale: tileSize);

                    return new Polygon(offset, vertices);
                }
            }

            return null;
        }

        protected override void DrawTileShape(RendererBatch batch, Color color, Point position)
        {
            var index = Map[position];

            if (index != Map.Empty)
            {
                Tile tile;

                if (Tiles.TryGetValue(index, out tile) && tile.Color.HasValue)
                    color = tile.Color.Value;
            }

            base.DrawTileShape(batch, color, position);
        }

        protected override void OnMapResized()
        {
            AttributeMap.Resize(MapSize);
        }

        public void DefineTile(int index, Color? color = null)
        {
            var vertices = new Vector2[]
            {
                Vector2.Zero,
                Vector2.UnitX,
                Vector2.One,
                Vector2.UnitY
            };

            DefineTile(index, vertices, Vector2.One, color);
        }

        public void DefineTile(int index, Vector2[] vertices, Color? color = null)
        {
            DefineTile(index, vertices, TileSize.ToVector2(), color);
        }

        public void DefineTile(int index, Vector2[] vertices, Vector2 size, Color? color = null)
        {
            var truncated = vertices.Truncate(size);

            if (truncated.Length < 3)
                throw new ArgumentException("Invalid tile shape provided.");

            var tile = new Tile()
            {
                Vertices = truncated,
                Color = color
            };

            if (Tiles.ContainsKey(index))
                Tiles[index] = tile;
            else Tiles.Add(index, tile);
        }

        public override void Reset(bool hardReset = false)
        {
            if (hardReset)
                Tiles.Clear();

            AttributeMap.Clear();
            base.Reset(hardReset);
        }

        public override Collider Clone()
        {
            var collider = new ComplexMapCollider<T>(Position, MapSize, TileSize);
            collider.Tiles = new Dictionary<int, Tile>(Tiles);
            collider.Map = Map.Clone();
            return collider;
        }

        public bool Contains(float x, float y, T attributes)
        {
            return Contains(new Vector2(x, y), attributes);
        }

        public bool Contains(Vector2 point, T attributes)
        {
            if (IsCollidable)
            {
                var location = point
                    .SnapToGrid(TileSize.ToVector2(), AbsolutePosition)
                    .ToPoint();

                var tile = GetTileShape(location);
                var collide = tile?.Contains(point) ?? false;
                return collide && HasTags(location, attributes);
            }

            return false;
        }

        public bool Raycast(
            float x1,
            float y1,
            float x2,
            float y2,
            T attributes,
            out Raycast hit)
        {
            return Raycast(new Vector2(x1, y1), new Vector2(x2, y2), attributes, out hit);
        }

        public bool Raycast(Vector2 start, Vector2 end, T attributes, out Raycast hit)
        {
            hit = default;

            if (IsCollidable)
            {
                var tileSize = TileSize.ToVector2();
                var gStart = start.SnapToGrid(tileSize, AbsolutePosition);
                var gEnd = end.SnapToGrid(tileSize, AbsolutePosition);

                foreach (var location in PlotLine(gStart, gEnd))
                {
                    var tile = GetTileShape(location);
                    var hitDetected = tile?.Raycast(start, end, out hit) ?? false;
                    if (hitDetected && HasTags(location, attributes))
                        return true;
                }
            }

            return false;
        }

        public bool Overlaps(Shape shape, T attributes)
        {
            if (IsCollidable && shape != null)
            {
                var region = shape.Bounds
                    .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                foreach (var location in PlotRegion(region))
                {
                    var tile = GetTileShape(location);
                    var overlaps = tile?.Overlaps(shape) ?? false;
                    if (overlaps && HasTags(location, attributes))
                        return true;
                }
            }

            return false;
        }

        public bool Overlaps(Shape shape, T attributes, out CollisionResult result)
        {
            result = new CollisionResult(this);

            var collide = false;

            if (IsCollidable && shape != null)
            {
                var region = shape.Bounds
                    .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                foreach (var location in PlotRegion(region))
                {
                    Manifold hit = default;
                    var tile = GetTileShape(location);
                    var overlaps = tile?.Overlaps(shape, out hit) ?? false;

                    if (overlaps && HasTags(location, attributes))
                    {
                        hit.Normal = -hit.Normal;
                        result.Add(hit);
                        collide = true;
                    }
                }
            }

            return collide;
        }

        public bool Overlaps(Collider collider, T attributes)
        {
            var canCollide = collider != null
                && collider != this
                && IsCollidable
                && collider.IsCollidable;

            if (canCollide)
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
                        if (overlaps && HasTags(location, attributes))
                            return true;
                    }
                }
            }

            return false;
        }

        public bool Overlaps(Collider collider, T attributes, out CollisionResult result)
        {
            result = new CollisionResult(this);

            var canCollide = collider != null
                && collider != this
                && IsCollidable
                && collider.IsCollidable;

            if (canCollide)
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

                        if (overlaps && HasTags(location, attributes))
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

        public bool Overlaps(Entity entity, T attributes)
        {
            return entity != null
                && entity != Entity
                && Overlaps(entity.Collider, attributes);
        }

        public bool Overlaps(Entity entity, T attributes, out CollisionResult result)
        {
            result = new CollisionResult(this);

            return entity != null
                && entity != Entity
                && Overlaps(entity.Collider, attributes, out result);
        }

        public bool ContainsTags(float x, float y, T attributes)
        {
            return ContainsTags(new Vector2(x, y), attributes);
        }

        public bool ContainsTags(Vector2 point, T attributes)
        {
            if (IsCollidable)
            {
                var location = point
                    .SnapToGrid(TileSize.ToVector2(), AbsolutePosition)
                    .ToPoint();

                return HasTags(location, attributes);
            }

            return false;
        }

        public bool ContainsTags(float x1, float y1, float x2, float y2, T attributes)
        {
            return ContainsTags(new Vector2(x1, y1), new Vector2(x2, y2), attributes);
        }

        public bool ContainsTags(Vector2 start, Vector2 end, T attributes)
        {
            if (IsCollidable)
            {
                var tileSize = TileSize.ToVector2();
                var gStart = start.SnapToGrid(tileSize, AbsolutePosition);
                var gEnd = end.SnapToGrid(tileSize, AbsolutePosition);

                foreach (var location in PlotLine(gStart, gEnd))
                    if (HasTags(location, attributes))
                        return true;
            }

            return false;
        }

        public bool ContainsTags(Shape shape, T attributes)
        {
            if (IsCollidable && shape != null)
            {
                var region = shape.Bounds
                    .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                foreach (var location in PlotRegion(region))
                    if (HasTags(location, attributes))
                        return true;
            }

            return false;
        }

        public bool ContainsTags(Collider collider, T attributes)
        {
            var canCollide = collider != null
                && collider != this
                && IsCollidable
                && collider.IsCollidable;

            if (canCollide)
            {
                if (collider is ShapeCollider)
                {
                    var shape = (collider as ShapeCollider).Shape;
                    var region = shape.Bounds
                        .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                    foreach (var location in PlotRegion(region))
                        if (HasTags(location, attributes))
                            return true;
                }
            }

            return false;
        }

        public bool ContainsTags(Entity entity, T attributes)
        {
            return entity != null
                && entity != Entity
                && ContainsTags(entity.Collider, attributes);
        }

        public void Fill(int tile, int x, int y)
        {
            Fill(tile, new Point(x, y), new Point(1, 1));
        }

        public void Fill(int tile, Point location)
        {
            Fill(tile, location, new Point(1, 1));
        }

        public void Fill(int tile, int x, int y, int width, int height)
        {
            Fill(tile, new Point(x, y), new Point(width, height));
        }

        public void Fill(int tile, Point location, Point size)
        {
            var min = location.Max(Point.Zero);
            var max = (location + size).Min(MapSize);
            var drawSize = max - min;

            for (int i = 0; i < drawSize.X * drawSize.Y; i++)
            {
                var x = min.X + (i % drawSize.X);
                var y = min.Y + (i / drawSize.X);
                Map[x, y] = tile;
            }
        }

        public void Populate(int[,] tiles, int offsetX, int offsetY)
        {
            Populate(tiles, new Point(offsetX, offsetY));
        }

        public void Populate(int[,] tiles, Point offset)
        {
            var columns = tiles.GetLength(0);
            var rows = tiles.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;
                var mapOffset = offset + new Point(x, y);
                Map[mapOffset] = tiles[x, y];
            }
        }

        public void Overlay(int[,] tiles, int offsetX, int offsetY)
        {
            Overlay(tiles, new Point(offsetX, offsetY));
        }

        public void Overlay(int[,] tiles, Point offset)
        {
            var columns = tiles.GetLength(0);
            var rows = tiles.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;

                if (!tiles[x, y].Equals(Map.Empty))
                {
                    var mapOffset = offset + new Point(x, y);
                    Map[mapOffset] = tiles[x, y];
                }
            }
        }

        public ComplexTile<T> GetTileAt(int x, int y)
        {
            return GetTileAt(new Point(x, y));
        }

        public ComplexTile<T> GetTileAt(Point location)
        {
            return new ComplexTile<T>(
                location,
                GetTileShape(location),
                AttributeMap[location]);
        }

        public ComplexTile<T> GetTile(float x, float y)
        {
            return GetTile(new Vector2(x, y));
        }

        public ComplexTile<T> GetTile(Vector2 point)
        {
            var location = point
                .SnapToGrid(TileSize.ToVector2(), AbsolutePosition)
                .ToPoint();

            return new ComplexTile<T>(
                location,
                GetTileShape(location),
                AttributeMap[location]);
        }

        public IEnumerable<ComplexTile<T>> GetTiles(float x1, float y1, float x2, float y2)
        {
            return GetTiles(new Vector2(x1, y1), new Vector2(x2, y2));
        }

        public IEnumerable<ComplexTile<T>> GetTiles(Vector2 start, Vector2 end)
        {
            var tileSize = TileSize.ToVector2();

            start = start.SnapToGrid(tileSize, AbsolutePosition);
            end = end.SnapToGrid(tileSize, AbsolutePosition);

            foreach (var location in PlotLine(start, end))
            {
                yield return new ComplexTile<T>(
                    location,
                    GetTileShape(location),
                    AttributeMap[location]);
            }
        }

        public IEnumerable<ComplexTile<T>> GetTiles(Shape shape)
        {
            var region = shape.Bounds
                .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

            foreach (var location in PlotRegion(region))
            {
                yield return new ComplexTile<T>(
                    location,
                    GetTileShape(location),
                    AttributeMap[location]);
            }
        }

        public IEnumerable<ComplexTile<T>> GetTiles(Collider collider)
        {
            if (collider != null && collider != this)
            {
                if (collider is ShapeCollider)
                {
                    var region = collider.Bounds
                        .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                    foreach (var location in PlotRegion(region))
                    {
                        yield return new ComplexTile<T>(
                            location,
                            GetTileShape(location),
                            AttributeMap[location]);
                    }
                }
            }
        }

        public IEnumerable<ComplexTile<T>> GetTiles(Entity entity)
        {
            return entity != null && entity != Entity
                ? GetTiles(entity.Collider) : null;
        }

        #region Attribute Map
        public void Tag(T attributes, int x, int y)
        {
            Tag(attributes, new Point(x, y), new Point(1, 1));
        }

        public void Tag(T attributes, Point location)
        {
            Tag(attributes, location, new Point(1, 1));
        }

        public void Tag(T attributes, int x, int y, int width, int height)
        {
            Tag(attributes, new Point(x, y), new Point(width, height));
        }

        public void Tag(T attributes, Point location, Point size)
        {
            var min = location.Max(Point.Zero);
            var max = (location + size).Min(MapSize);
            var drawSize = max - min;

            for (int i = 0; i < drawSize.X * drawSize.Y; i++)
            {
                var x = min.X + (i % drawSize.X);
                var y = min.Y + (i / drawSize.X);
                AttributeMap[x, y] = attributes;
            }
        }

        public void Tag(T[,] attributes, int offsetX, int offsetY)
        {
            Tag(attributes, new Point(offsetX, offsetY));
        }

        public void Tag(T[,] attributes, Point offset)
        {
            var columns = attributes.GetLength(0);
            var rows = attributes.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;
                var mapOffset = offset + new Point(x, y);
                AttributeMap[mapOffset] = attributes[x, y];
            }
        }

        bool HasTags(Point location, T attributes)
        {
            if (!attributes.Equals(Map.Empty))
            {
                var enumValue = (Enum)Enum.ToObject(typeof(T), AttributeMap[location]);
                var searchValue = (Enum)(object)attributes;
                var hasFlagAttribute = typeof(T).IsDefined(typeof(FlagsAttribute), false);

                return hasFlagAttribute
                    ? enumValue.HasFlag(searchValue)
                    : enumValue.Equals(searchValue);
            }

            return false;
        }
        #endregion

        #region Tile
        struct Tile
        {
            public Vector2[] Vertices { get; set; }

            public Color? Color { get; set; }
        }
        #endregion
    }

    public struct ComplexTile<T>
    {
        public Point Location { get; internal set; }

        public int X => Location.X;

        public int Y => Location.Y;

        public Shape Shape { get; internal set; }

        public T Attributes { get; internal set; }

        public bool HasAttributes => !Attributes.Equals(default(T));

        internal ComplexTile(Point location, Shape shape, T attributes)
            : this()
        {
            Location = location;
            Shape = shape;
            Attributes = attributes;
        }
    }
}
