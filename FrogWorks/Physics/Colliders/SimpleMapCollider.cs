using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public sealed class SimpleMapCollider
        : TileMapCollider, IMapModifier<bool>, IMapAccessor<SimpleTile>
    {
        public SimpleMapCollider(
            int mapWidth, int mapHeight,
            int tileWidth, int tileHeight)
            : this(
                  Vector2.Zero,
                  new Point(mapWidth, mapHeight),
                  new Point(tileWidth, tileHeight))
        {
        }

        public SimpleMapCollider(
            float x, float y,
            int mapWidth, int mapHeight,
            int tileWidth, int tileHeight)
            : this(
                  new Vector2(x, y),
                  new Point(mapWidth, mapHeight),
                  new Point(tileWidth, tileHeight))
        {
        }

        public SimpleMapCollider(Point mapSize, Point tileSize)
            : this(Vector2.Zero, mapSize, tileSize)
        {
        }

        public SimpleMapCollider(
            Vector2 position,
            Point mapSize,
            Point tileSize)
            : base(position, mapSize, tileSize)
        {
        }

        protected override Shape GetTileShape(Point position)
        {
            var isSolid = Convert.ToBoolean(Map[position]);

            if (isSolid)
            {
                var tileSize = TileSize.ToVector2();
                var location = AbsolutePosition + position.ToVector2() * tileSize;
                return new Box(location, tileSize);
            }

            return null;
        }

        public override Collider Clone()
        {
            var collider = new SimpleMapCollider(Position, MapSize, TileSize);
            collider.Map = Map.Clone();
            return collider;
        }

        public void Fill(bool tile, int x, int y)
        {
            Fill(tile, new Point(x, y), new Point(1, 1));
        }

        public void Fill(bool tile, Point location)
        {
            Fill(tile, location, new Point(1, 1));
        }

        public void Fill(bool tile, int x, int y, int width, int height)
        {
            Fill(tile, new Point(x, y), new Point(width, height));
        }

        public void Fill(bool tile, Point location, Point size)
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

        public void Populate(bool[,] tiles, int offsetX, int offsetY)
        {
            Populate(tiles, new Point(offsetX, offsetY));
        }

        public void Populate(bool[,] tiles, Point offset)
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

        public void Overlay(bool[,] tiles, int offsetX, int offsetY)
        {
            Overlay(tiles, new Point(offsetX, offsetY));
        }

        public void Overlay(bool[,] tiles, Point offset)
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

        public SimpleTile GetTileAt(int x, int y)
        {
            return GetTileAt(new Point(x, y));
        }

        public SimpleTile GetTileAt(Point location)
        {
            return new SimpleTile(location, Map[location]);
        }

        public SimpleTile GetTile(float x, float y)
        {
            return GetTile(new Vector2(x, y));
        }

        public SimpleTile GetTile(Vector2 point)
        {
            var location = point
                .SnapToGrid(TileSize.ToVector2(), AbsolutePosition)
                .ToPoint();

            return new SimpleTile(location, Map[location]);
        }

        public IEnumerable<SimpleTile> GetTiles(float x1, float y1, float x2, float y2)
        {
            return GetTiles(new Vector2(x1, y1), new Vector2(x2, y2));
        }

        public IEnumerable<SimpleTile> GetTiles(Vector2 start, Vector2 end)
        {
            var tileSize = TileSize.ToVector2();

            start = start.SnapToGrid(tileSize, AbsolutePosition);
            end = end.SnapToGrid(tileSize, AbsolutePosition);

            foreach (var location in PlotLine(start, end))
                yield return new SimpleTile(location, Map[location]);
        }

        public IEnumerable<SimpleTile> GetTiles(Shape shape)
        {
            var region = shape.Bounds
                .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

            foreach (var location in PlotRegion(region))
                yield return new SimpleTile(location, Map[location]);
        }

        public IEnumerable<SimpleTile> GetTiles(Collider collider)
        {
            if (collider != null && collider != this)
            {
                if (collider is ShapeCollider)
                {
                    var region = collider.Bounds
                        .SnapToGrid(TileSize.ToVector2(), AbsolutePosition);

                    foreach (var location in PlotRegion(region))
                        yield return new SimpleTile(location, Map[location]);
                }
            }
        }

        public IEnumerable<SimpleTile> GetTiles(Entity entity)
        {
            return entity != null && entity != Entity
                ? GetTiles(entity.Collider) : null;
        }
    }

    public struct SimpleTile
    {
        public Point Location { get; internal set; }

        public int X => Location.X;

        public int Y => Location.Y;

        public bool IsSolid { get; internal set; }

        internal SimpleTile(Point location, int index)
            : this()
        {
            Location = location;
            IsSolid = Convert.ToBoolean(index);
        }
    }
}
