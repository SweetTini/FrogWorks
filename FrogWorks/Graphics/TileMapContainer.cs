using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public sealed class TileMapContainer
    {
        private Dictionary<string, TileMap> _layers;

        public int Columns { get; internal set; }

        public int Rows { get; internal set; }

        public int TileWidth { get; internal set; }

        public int TileHeight { get; internal set; }

        internal TileMapContainer()
        {
            _layers = new Dictionary<string, TileMap>();
        }

        internal void AddLayer(string name, TileMap tileMap) => _layers.Add(name, tileMap);

        public TileMap GetLayer(string name)
        {
            TileMap tileMap;
            _layers.TryGetValue(name, out tileMap);
            return tileMap;
        }
    }
}
