using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public sealed class TileMapContainer
    {
        public Point Size { get; internal set; }

        public int Columns => Size.X;

        public int Rows => Size.Y;

        public Point TileSize { get; internal set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        internal Dictionary<string, TileMap> TileLayers { get; private set; }

        internal Dictionary<string, int[,]> DataLayers { get; private set; }

        internal TileMapContainer()
        {
            TileLayers = new Dictionary<string, TileMap>();
            DataLayers = new Dictionary<string, int[,]>();
        }

        public TileMap GetTileLayer(string name)
        {
            TileMap tileLayer;
            TileLayers.TryGetValue(name, out tileLayer);
            return tileLayer;
        }

        public void ProcessDataLayer(string name, Action<int, int, int, int, int> processAction)
        {
            int[,] dataLayer;

            if (DataLayers.TryGetValue(name, out dataLayer))
            {
                for (int i = 0; i < Size.X * Size.Y; i++)
                {
                    var x = i % TileSize.X;
                    var y = i / TileSize.X;
                    var data = dataLayer[x, y];
                    if (data == 0) continue;

                    processAction(x, y, TileSize.X, TileSize.Y, dataLayer[x, y]);
                }
            }
        }
    }
}
