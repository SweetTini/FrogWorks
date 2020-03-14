using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public sealed class SimpleMapCollider : TileMapCollider, IMapManager<bool>
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
    }
}
