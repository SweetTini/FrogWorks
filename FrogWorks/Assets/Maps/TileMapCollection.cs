using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public sealed class TileMapCollection
    {
        static Dictionary<string, TileMapCollection> Cache { get; } = 
            new Dictionary<string, TileMapCollection>();

        public Point Size { get; internal set; }

        public int Columns => Size.X;

        public int Rows => Size.Y;

        public Point TileSize { get; internal set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public Color BackgroundColor { get; internal set; }

        public ReadOnlyCollection<TileMapCollectionTileMap> TileMaps { get; private set; }

        public ReadOnlyCollection<TileMapCollectionTile> UnusedTiles { get; private set; }

        public ReadOnlyCollection<TileMapCollectionObject> Objects { get; private set; }

        internal List<TileMapCollectionTileMap> InternalTileMaps { get; set; }

        internal List<TileMapCollectionTile> InternalUnusedTiles { get; set; }

        internal List<TileMapCollectionObject> InternalObjects { get; set; }

        internal TileMapCollection()
            : base()
        {
            InternalTileMaps = new List<TileMapCollectionTileMap>();
            InternalUnusedTiles = new List<TileMapCollectionTile>();
            InternalObjects = new List<TileMapCollectionObject>();
            TileMaps = new ReadOnlyCollection<TileMapCollectionTileMap>(InternalTileMaps);
            UnusedTiles = new ReadOnlyCollection<TileMapCollectionTile>(InternalUnusedTiles);
            Objects = new ReadOnlyCollection<TileMapCollectionObject>(InternalObjects);
        }

        #region Static Methods
        internal static bool TryGetFromCache(string filePath, Func<string, TileMapCollection> loadFunc, 
            out TileMapCollection collection)
        {
            if (!Cache.TryGetValue(filePath, out collection))
            {
                collection = loadFunc?.Invoke(filePath) ?? null;
                if (collection != null) Cache.Add(filePath, collection);
            }

            return collection != null;
        }

        public static void Dispose()
        {
            Cache.Clear();
        }
        #endregion
    }

    public class TileMapCollectionTileMap
    {
        public string GroupName { get; internal set; }

        public TileMap Component { get; internal set; }

        public ReadOnlyDictionary<string, object> Properties { get; private set; }

        internal Dictionary<string, object> InternalProperties { get; set; }

        internal TileMapCollectionTileMap()
            : base()
        {
            InternalProperties = new Dictionary<string, object>();
            Properties = new ReadOnlyDictionary<string, object>(InternalProperties);
        }
    }

    public class TileMapCollectionTile
    {
        public int Gid { get; internal set; }

        public string GroupName { get; internal set; }

        public Point Position { get; internal set; }

        public int X => Position.X;

        public int Y => Position.Y;

        internal TileMapCollectionTile()
            : base()
        {
        }
    }

    public class TileMapCollectionObject
    {
        public string Name { get; internal set; }

        public string Type { get; internal set; }

        public Rectangle Region { get; internal set; }

        public Point Position => Region.Location;

        public int X => Region.X;

        public int Y => Region.Y;

        public Point Size => Region.Size;

        public int Width => Region.Width;

        public int Height => Region.Height;

        public ReadOnlyDictionary<string, object> Properties { get; private set; }

        internal Dictionary<string, object> InternalProperties { get; set; }

        internal TileMapCollectionObject()
            : base()
        {
            InternalProperties = new Dictionary<string, object>();
            Properties = new ReadOnlyDictionary<string, object>(InternalProperties);
        }
    }
}
