using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public sealed class ComplexMapCollider : TileMapCollider, IMapManager<int>
    {
        public ComplexMapCollider(
            int mapWidth, int mapHeight,
            int tileWidth, int tileHeight)
            : this(
                  Vector2.Zero,
                  new Point(mapWidth, mapHeight),
                  new Point(tileWidth, tileHeight))
        {
        }

        public ComplexMapCollider(
            float x, float y,
            int mapWidth, int mapHeight,
            int tileWidth, int tileHeight)
            : this(
                  new Vector2(x, y),
                  new Point(mapWidth, mapHeight),
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
        }

        protected override Shape GetTileShape(Point position)
        {
            throw new NotImplementedException();
        }

        public override Collider Clone()
        {
            var collider = new ComplexMapCollider(Position, MapSize, TileSize);
            collider.Map = Map.Clone();
            return collider;
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
    }
}
