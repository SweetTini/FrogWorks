using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public sealed class TileMapContainer
    {
        private Dictionary<string, TileMap> _tileLayers;
        private Dictionary<string, int[,]> _dataLayers;

        public Point Size { get; internal set; }

        public int Columns => Size.X;

        public int Rows => Size.Y;

        public Point TileSize { get; internal set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public ReadOnlyDictionary<string, TileMap> TileLayers { get; private set; }

        public ReadOnlyDictionary<string, int[,]> DataLayers { get; private set; }

        internal TileMapContainer()
        {
            _tileLayers = new Dictionary<string, TileMap>();
            _dataLayers = new Dictionary<string, int[,]>();

            TileLayers = new ReadOnlyDictionary<string, TileMap>(_tileLayers);
            DataLayers = new ReadOnlyDictionary<string, int[,]>(_dataLayers);
        }

        internal void AddTileLayer(string name, TileMap tileMap) => _tileLayers.Add(name, tileMap);

        internal void AddDataLayer(string name, int[,] data) => _dataLayers.Add(name, data);

        public void ProcessDataLayer(string name, Action<int, int, int, int, int> processAction)
        {
            int[,] dataLayer;

            if (DataLayers.TryGetValue(name, out dataLayer))
            {
                for (int i = 0; i < Size.X * Size.Y; i++)
                {
                    var x = i % Size.X;
                    var y = i / Size.X;
                    var data = dataLayer[x, y];
                    if (data == 0) continue;

                    processAction(x, y, TileSize.X, TileSize.Y, dataLayer[x, y]);
                }
            }
        }
    }
}
