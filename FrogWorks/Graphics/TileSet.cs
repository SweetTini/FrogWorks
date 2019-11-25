using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class TileSet
    {
        private Texture[,] _tiles;

        public Texture this[int x, int y]
        {
            get { return _tiles.WithinRange(x, y) ? _tiles[x, y] : null; }
        }

        public Texture this[int index]
        {
            get
            {
                var x = index % _tiles.GetLength(0);
                var y = index / _tiles.GetLength(0);
                return _tiles.WithinRange(x, y) ? _tiles[x, y] : null;
            }
        }

        public int Count => _tiles.GetLength(0) * _tiles.GetLength(1);

        public Texture Texture { get; private set; }

        public Point TileSize { get; private set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public TileSet(Texture texture, Point tileSize)
        {
            Texture = texture;
            TileSize = tileSize.Abs();

            var columns = texture.Width / TileWidth;
            var rows = texture.Height / TileHeight;
            var textures = Texture.Split(Texture, TileWidth, TileHeight);

            _tiles = new Texture[columns, rows];

            for (int i = 0; i < textures.Length; i++)
            {
                var x = i % columns;
                var y = i / columns;
                _tiles[x, y] = textures[i];
            }
        }

        public TileSet(Texture texture, int tileWidth, int tileHeight)
            : this(texture, new Point(tileWidth, tileHeight))
        {
        }

        public TileSet Clone()
        {
            return new TileSet(Texture, TileSize);
        }
    }
}
