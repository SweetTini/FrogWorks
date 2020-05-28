using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public sealed class TaggedMapCollider<T> :
        TileMapCollider,
        ITaggedMapCollisionHandler<T>,
        IMapModifier<T>,
        IMapAccessor<TaggedTile<T>>
        where T : struct
    {
        Dictionary<T, Color> Colors { get; set; }

        public TaggedMapCollider(
            int columns, int rows,
            int tileWidth, int tileHeight)
            : this(
                  Vector2.Zero,
                  new Point(columns, rows),
                  new Point(tileWidth, tileHeight))
        {
        }

        public TaggedMapCollider(
            float x, float y,
            int columns, int rows,
            int tileWidth, int tileHeight)
            : this(
                  new Vector2(x, y),
                  new Point(columns, rows),
                  new Point(tileWidth, tileHeight))
        {
        }

        public TaggedMapCollider(Point mapSize, Point tileSize)
            : this(Vector2.Zero, mapSize, tileSize)
        {
        }

        public TaggedMapCollider(
            Vector2 position,
            Point mapSize,
            Point tileSize)
            : base(position, mapSize, tileSize)
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("Generic type must be an Enum.");

            Colors = new Dictionary<T, Color>();
        }

        protected override Shape GetTileShape(Point position)
        {
            var index = Map[position];

            if (index != Map.Empty)
            {
                var tileSize = TileSize.ToVector2();
                var location = AbsolutePosition + position.ToVector2() * tileSize;
                return new Box(location, tileSize);
            }

            return null;
        }

        protected override void DrawTileShape(RendererBatch batch, Color color, Point position)
        {
            var index = Map[position];

            if (typeof(T).IsEnum)
            {
                var isFlag = typeof(T).IsDefined(typeof(FlagsAttribute), false);

                if (isFlag)
                {
                    var first = true;

                    foreach (var pair in Colors)
                    {
                        var tile = (Enum)Enum.ToObject(typeof(T), index);
                        var tileToFind = (Enum)Enum.ToObject(typeof(T), pair.Key);

                        if (tile.HasFlag(tileToFind))
                        {
                            color = !first
                                ? Color.Lerp(color, pair.Value, .5f)
                                : pair.Value;

                            first = false;
                        }
                    }
                }
                else
                {
                    var originalColor = color;
                    var tile = (T)Enum.ToObject(typeof(T), index);

                    if (!Colors.TryGetValue(tile, out color))
                        color = originalColor;
                }
            }

            base.DrawTileShape(batch, color, position);
        }

        public void DefineColor(T tile, Color color)
        {
            if (Colors.ContainsKey(tile))
                Colors[tile] = color;
            else Colors.Add(tile, color);
        }

        public override void Reset(bool hardReset = false)
        {
            if (hardReset)
                Colors.Clear();

            base.Reset(hardReset);
        }

        public override Collider Clone()
        {
            var collider = new TaggedMapCollider<T>(Position, MapSize, TileSize);
            collider.Colors = new Dictionary<T, Color>(Colors);
            collider.Map = Map.Clone();
            return collider;
        }

        public bool Contains(Vector2 point, T attributes)
        {
            if (IsCollidable)
            {
                var location = point.ToGrid(TileSize.ToVector2(), AbsolutePosition).ToPoint();
                var tile = GetTileShape(location);
                var collide = tile?.Contains(point) ?? false;
                return collide && HasAttributes(location, attributes);
            }

            return false;
        }

        public bool Contains(float x, float y, T attributes)
        {
            return Contains(new Vector2(x, y), attributes);
        }

        public bool Contains(Vector2 point, T attributes, out Vector2 depth)
        {
            depth = default;

            if (IsCollidable)
            {
                var location = point.ToGrid(TileSize.ToVector2(), AbsolutePosition).ToPoint();
                var tile = GetTileShape(location);
                var collide = tile?.Contains(point, out depth) ?? false;
                return collide && HasAttributes(location, attributes);
            }

            return false;
        }

        public bool Contains(float x, float y, T attributes, out Vector2 depth)
        {
            return Contains(new Vector2(x, y), attributes, out depth);
        }

        public bool CastRay(Vector2 origin, Vector2 normal, float distance, T attributes)
        {
            return CastRay(origin, normal, distance, attributes, out _);
        }

        public bool CastRay(
            Vector2 origin,
            Vector2 normal,
            float distance,
            T attributes,
            out Raycast hit)
        {
            hit = default;

            if (IsCollidable)
            {
                var tileSize = TileSize.ToVector2();
                var gStart = origin.ToGrid(tileSize, AbsolutePosition);
                var gEnd = (origin + normal * distance).ToGrid(tileSize, AbsolutePosition);

                foreach (var location in PlotLine(gStart, gEnd))
                {
                    var tile = GetTileShape(location);
                    var hitDetected = tile?.CastRay(origin, normal, distance, out hit) ?? false;
                    if (hitDetected && HasAttributes(location, attributes))
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
                    .ToGrid(TileSize.ToVector2(), AbsolutePosition);

                foreach (var location in PlotRegion(region))
                {
                    var tile = GetTileShape(location);
                    var overlaps = tile?.Overlaps(shape) ?? false;
                    if (overlaps && HasAttributes(location, attributes))
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
                    .ToGrid(TileSize.ToVector2(), AbsolutePosition);

                foreach (var location in PlotRegion(region))
                {
                    Manifold hit = default;
                    var tile = GetTileShape(location);
                    var overlaps = tile?.Overlaps(shape, out hit) ?? false;

                    if (overlaps && HasAttributes(location, attributes))
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
                        .ToGrid(TileSize.ToVector2(), AbsolutePosition);

                    foreach (var location in PlotRegion(region))
                    {
                        var tile = GetTileShape(location);
                        var overlaps = tile?.Overlaps(shape) ?? false;
                        if (overlaps && HasAttributes(location, attributes))
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
                        .ToGrid(TileSize.ToVector2(), AbsolutePosition);

                    foreach (var location in PlotRegion(region))
                    {
                        Manifold hit = default;
                        var tile = GetTileShape(location);
                        var overlaps = tile?.Overlaps(shape, out hit) ?? false;

                        if (overlaps && HasAttributes(location, attributes))
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

        public void Fill(T tile, int x, int y)
        {
            Fill(tile, new Point(x, y), new Point(1, 1));
        }

        public void Fill(T tile, Point location)
        {
            Fill(tile, location, new Point(1, 1));
        }

        public void Fill(T tile, int x, int y, int width, int height)
        {
            Fill(tile, new Point(x, y), new Point(width, height));
        }

        public void Fill(T tile, Point location, Point size)
        {
            var min = location.Max(Point.Zero);
            var max = (location + size).Min(MapSize);
            var drawSize = max - min;

            for (int i = 0; i < drawSize.X * drawSize.Y; i++)
            {
                var x = min.X + (i % drawSize.X);
                var y = min.Y + (i / drawSize.X);
                Map[x, y] = Convert.ToInt32(tile);
            }
        }

        public void Populate(T[,] tiles, int offsetX, int offsetY)
        {
            Populate(tiles, new Point(offsetX, offsetY));
        }

        public void Populate(T[,] tiles, Point offset)
        {
            var columns = tiles.GetLength(0);
            var rows = tiles.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;
                var mapOffset = offset + new Point(x, y);
                Map[mapOffset] = Convert.ToInt32(tiles[x, y]);
            }
        }

        public void Overlay(T[,] tiles, int offsetX, int offsetY)
        {
            Overlay(tiles, new Point(offsetX, offsetY));
        }

        public void Overlay(T[,] tiles, Point offset)
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
                    Map[mapOffset] = Convert.ToInt32(tiles[x, y]);
                }
            }
        }

        public TaggedTile<T> GetTileAt(int x, int y)
        {
            return GetTileAt(new Point(x, y));
        }

        public TaggedTile<T> GetTileAt(Point location)
        {
            return new TaggedTile<T>(location, Map[location]);
        }

        public TaggedTile<T> GetTile(float x, float y)
        {
            return GetTile(new Vector2(x, y));
        }

        public TaggedTile<T> GetTile(Vector2 point)
        {
            var location = point
                .ToGrid(TileSize.ToVector2(), AbsolutePosition)
                .ToPoint();

            return new TaggedTile<T>(location, Map[location]);
        }

        public IEnumerable<TaggedTile<T>> GetTiles(float x1, float y1, float x2, float y2)
        {
            return GetTiles(new Vector2(x1, y1), new Vector2(x2, y2));
        }

        public IEnumerable<TaggedTile<T>> GetTiles(Vector2 start, Vector2 end)
        {
            var tileSize = TileSize.ToVector2();

            start = start.ToGrid(tileSize, AbsolutePosition);
            end = end.ToGrid(tileSize, AbsolutePosition);

            foreach (var location in PlotLine(start, end))
                yield return new TaggedTile<T>(location, Map[location]);
        }

        public IEnumerable<TaggedTile<T>> GetTiles(Shape shape)
        {
            var region = shape.Bounds
                .ToGrid(TileSize.ToVector2(), AbsolutePosition);

            foreach (var location in PlotRegion(region))
                yield return new TaggedTile<T>(location, Map[location]);
        }

        public IEnumerable<TaggedTile<T>> GetTiles(Collider collider)
        {
            if (collider != null && collider != this)
            {
                if (collider is ShapeCollider)
                {
                    var region = collider.Bounds
                        .ToGrid(TileSize.ToVector2(), AbsolutePosition);

                    foreach (var location in PlotRegion(region))
                        yield return new TaggedTile<T>(location, Map[location]);
                }
            }
        }

        public IEnumerable<TaggedTile<T>> GetTiles(Entity entity)
        {
            return entity != null && entity != Entity
                ? GetTiles(entity.Collider) : null;
        }

        bool HasAttributes(Point location, T attributes)
        {
            if (!attributes.Equals(Map.Empty))
            {
                var enumValue = (Enum)Enum.ToObject(typeof(T), Map[location]);
                var searchValue = (Enum)(object)attributes;
                var hasFlagAttribute = typeof(T).IsDefined(typeof(FlagsAttribute), false);

                return hasFlagAttribute
                    ? enumValue.HasFlag(searchValue)
                    : enumValue.Equals(searchValue);
            }

            return false;
        }
    }

    public interface ITaggedMapCollisionHandler<T>
    {
        bool Contains(Vector2 point, T attributes);

        bool Contains(float x, float y, T attributes);

        bool Contains(Vector2 point, T attributes, out Vector2 depth);

        bool Contains(float x, float y, T attributes, out Vector2 depth);

        bool CastRay(Vector2 start, Vector2 normal, float distance, T attributes);

        bool CastRay(Vector2 start, Vector2 normal, float distance, T attributes, out Raycast hit);

        bool Overlaps(Shape shape, T attributes);

        bool Overlaps(Shape shape, T attributes, out CollisionResult result);

        bool Overlaps(Collider collider, T attributes);

        bool Overlaps(Collider collider, T attributes, out CollisionResult result);

        bool Overlaps(Entity entity, T attributes);

        bool Overlaps(Entity entity, T attributes, out CollisionResult result);
    }

    public struct TaggedTile<T>
    {
        public Point Location { get; internal set; }

        public int X => Location.X;

        public int Y => Location.Y;

        public T Attributes { get; internal set; }

        public bool HasAttributes => !Attributes.Equals(default(T));

        public TaggedTile(Point location, int index)
            : this()
        {
            Location = location;
            Attributes = (T)Enum.ToObject(typeof(T), index);
        }
    }
}
