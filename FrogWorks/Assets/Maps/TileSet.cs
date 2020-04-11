using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public class TileSet
    {
        Texture[] _textures;
        Point _size;

        public Texture this[int x, int y]
        {
            get
            {
                var one = new Point(1, 1);

                return new Point(x, y).Between(Point.Zero, _size - one) 
                    ? _textures[x + (y * _size.X)] : null;
            }
        }

        public Texture this[int index]
        {
            get { return _textures.WithinRange(index) ? _textures[index] : null; }
        }

        public int Count => _textures.Length;

        public Texture Texture { get; private set; }

        public Point TileSize { get; private set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public TileSet(Texture texture, Point tileSize)
        {
            Texture = texture;
            TileSize = tileSize.Abs();

            _textures = Texture.Split(Texture, TileSize);
            _size = texture.Size.Divide(TileSize);
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
