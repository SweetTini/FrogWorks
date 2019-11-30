using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FrogWorks
{
    public sealed class TileMapContainer
    {
        private Dictionary<string, TileMap> _tileLayers;
        private Dictionary<string, int[,]> _dataLayers;
        private List<TileMapContainerObject> _objects;

        public Point Size { get; internal set; }

        public int Columns => Size.X;

        public int Rows => Size.Y;

        public Point TileSize { get; internal set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public ReadOnlyDictionary<string, TileMap> TileLayers { get; private set; }

        public ReadOnlyDictionary<string, int[,]> DataLayers { get; private set; }

        public ReadOnlyCollection<TileMapContainerObject> Objects { get; private set; }

        internal TileMapContainer()
        {
            _tileLayers = new Dictionary<string, TileMap>();
            _dataLayers = new Dictionary<string, int[,]>();
            _objects = new List<TileMapContainerObject>();

            TileLayers = new ReadOnlyDictionary<string, TileMap>(_tileLayers);
            DataLayers = new ReadOnlyDictionary<string, int[,]>(_dataLayers);
            Objects = new ReadOnlyCollection<TileMapContainerObject>(_objects);
        }

        internal void AddTileLayer(string name, TileMap tileMap) => _tileLayers.Add(name, tileMap);

        internal void AddDataLayer(string name, int[,] data) => _dataLayers.Add(name, data);

        internal void AddObject(TileMapContainerObject obj) => _objects.Add(obj);

        public void ProcessDataLayer(string name, Action<TileMapContainerDataInfo> processAction)
        {
            int[,] dataLayer;

            if (DataLayers.TryGetValue(name, out dataLayer))
            {
                for (int i = 0; i < Size.X * Size.Y; i++)
                {
                    var position = new Point(i % Size.X, i / Size.X);
                    var tileIndex = dataLayer[position.X, position.Y];
                    if (tileIndex == 0) continue;

                    var bounds = new Rectangle(position, new Point(1, 1));
                    var objects = _objects.Where(x => x.Bounds.Contains(bounds));

                    processAction(new TileMapContainerDataInfo(position, TileSize, tileIndex, objects));
                }
            }
        }
    }

    public sealed class TileMapContainerObject
    {
        Dictionary<string, object> _properties;

        public Point Position { get; internal set; }

        public int X => Position.X;

        public int Y => Position.Y;

        public Point Size { get; internal set; }

        public int Width => Size.X;

        public int Height => Size.Y;

        public ReadOnlyDictionary<string, object> Properties { get; private set; }

        internal Rectangle Bounds => new Rectangle(Position, Size);

        internal TileMapContainerObject()
        {
            _properties = new Dictionary<string, object>();
            Properties = new ReadOnlyDictionary<string, object>(_properties);
        }

        internal void AddProperty(string name, object value) => _properties.Add(name, value);
    }

    public sealed class TileMapContainerDataInfo
    {
        public Point Position { get; internal set; }

        public int X => Position.X;

        public int Y => Position.Y;

        public Point TileSize { get; internal set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public int TileIndex { get; internal set; }

        public ReadOnlyCollection<TileMapContainerObject> Objects { get; private set; }

        internal TileMapContainerDataInfo(Point position, 
                                          Point tileSize, 
                                          int tileIndex, 
                                          IEnumerable<TileMapContainerObject> objects)
        {
            Position = position;
            TileSize = tileSize;
            TileIndex = tileIndex;
            Objects = new ReadOnlyCollection<TileMapContainerObject>(objects.ToList());
        }
    }
}
