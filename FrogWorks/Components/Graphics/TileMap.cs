using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class TileMap : TiledGraphicsComponent
    {
        protected Map<Texture> Map { get; private set; }

        public Point Size => Map.Size;

        public int Columns => Map.Columns;

        public int Rows => Map.Rows;

        public TileMap(Point size, Point tileSize)
            : this(size, tileSize, false)
        {
        }

        public TileMap(int columns, int rows, int tileWidth, int tileHeight)
            : this(new Point(columns, rows), new Point(tileWidth, tileHeight), false)
        {
        }

        protected TileMap(Point size, Point tileSize, bool isEnabled)
            : base(isEnabled)
        {
            Map = new Map<Texture>(size);
            TileSize = tileSize;
        }

        protected override Texture GetTile(int x, int y)
        {
            if (WrapHorizontally) x = x.Mod(Columns);
            if (WrapVertically) y = y.Mod(Rows);

            return Map[x, y];
        }

        public void Populate(TileSet tileSet, int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            var columns = tiles.GetLength(0);
            var rows = tiles.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;

                Map[offsetX + x, offsetY + y] = tileSet[tiles[x, y]];
            }
        }

        public void Overlay(TileSet tileSet, int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            var columns = tiles.GetLength(0);
            var rows = tiles.GetLength(1);

            for (int i = 0; i < columns * rows; i++)
            {
                var x = i % columns;
                var y = i / columns;
                var index = tiles[x, y];

                if (index >= 0)
                    Map[offsetX + x, offsetY + y] = tileSet[index];
            }
        }

        public void Fill(Texture tile, Point location, Point size)
        {
            var from = location.Max(Point.Zero);
            var to = (location + size).Min(Size);
            var area = to - from;

            for (int i = 0; i < area.X * area.Y; i++)
            {
                var x = location.X + (i % area.X);
                var y = location.Y + (i / area.X);

                Map[x, y] = tile;
            }
        }

        public void Fill(Texture tile, int x, int y, int columns, int rows)
        {
            Fill(tile, new Point(x, y), new Point(columns, rows));
        }

        public void Clear() => Map.Clear();

        public void Resize(Point size) => Map.Resize(size);

        public void Resize(int columns, int rows) => Map.Resize(columns, rows);

        public void Resize(Point from, Point to) => Map.Resize(from, to);

        public void Resize(int x1, int y1, int x2, int y2) => Map.Resize(x1, y1, x2, y2);
    }
}
