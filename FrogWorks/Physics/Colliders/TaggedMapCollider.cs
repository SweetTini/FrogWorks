using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public sealed class TaggedMapCollider<T> : TileMapCollider, IMapManager<T>
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
    }
}
