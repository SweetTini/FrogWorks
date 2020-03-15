﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public sealed class TaggedMapCollider<T>
        : TileMapCollider, IMapModifier<T>, IMapAccessor<TaggedTile<T>>
        where T : struct
    {
        Dictionary<T, Color> Colors { get; set; }

        public TaggedMapCollider(
            int mapWidth, int mapHeight,
            int tileWidth, int tileHeight)
            : this(
                  Vector2.Zero,
                  new Point(mapWidth, mapHeight),
                  new Point(tileWidth, tileHeight))
        {
        }

        public TaggedMapCollider(
            float x, float y,
            int mapWidth, int mapHeight,
            int tileWidth, int tileHeight)
            : this(
                  new Vector2(x, y),
                  new Point(mapWidth, mapHeight),
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

            if (Enum.IsDefined(typeof(T), index))
            {
                var origColor = color;
                var tile = (T)Enum.ToObject(typeof(T), index);

                if (!Colors.TryGetValue(tile, out color))
                    color = origColor;
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

        public TaggedTile<T> GetTile(float x, float y)
        {
            return GetTile(new Vector2(x, y));
        }

        public TaggedTile<T> GetTile(Vector2 point)
        {
            var location = point
                .SnapToGrid(TileSize.ToVector2(), AbsolutePosition)
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

            start = start.SnapToGrid(tileSize, AbsolutePosition);
            end = end.SnapToGrid(tileSize, AbsolutePosition);

            foreach (var location in PlotLine(start, end))
                yield return new TaggedTile<T>(location, Map[location]);
        }

        public IEnumerable<TaggedTile<T>> GetTiles(Shape shape)
        {
            var region = shape.Bounds
                .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

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
                        .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

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