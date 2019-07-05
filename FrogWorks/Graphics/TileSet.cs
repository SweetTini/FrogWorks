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

        public Texture Texture { get; private set; }

        public int TileWidth { get; private set; }

        public int TileHeight { get; private set; }

        public TileSet(Texture texture, int tileWidth, int tileHeight)
        {
            Texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

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

        public TileSet Clone()
        {
            return new TileSet(Texture, TileWidth, TileHeight);
        }
    }
}
