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

        public TileMapCollectionProperties Properties { get; private set; }

        internal Dictionary<string, object> InternalProperties { get; set; }

        internal TileMapCollectionTileMap()
            : base()
        {
            InternalProperties = new Dictionary<string, object>();
            Properties = new TileMapCollectionProperties(InternalProperties);
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

        public TileMapCollectionProperties Properties { get; private set; }

        internal Dictionary<string, object> InternalProperties { get; set; }

        internal TileMapCollectionObject()
            : base()
        {
            InternalProperties = new Dictionary<string, object>();
            Properties = new TileMapCollectionProperties(InternalProperties);
        }
    }

    public class TileMapCollectionProperties
        : ReadOnlyDictionary<string, object>
    {
        public TileMapCollectionProperties(IDictionary<string, object> dictionary) 
            : base(dictionary)
        {
        }

        public int GetAsInt32(string key, int defaultValue = 0)
        {
            object result;
            TryGetValue(key, out result);
            return (int)(result ?? defaultValue);
        }

        public float GetAsSingle(string key, float defaultValue = 0f)
        {
            object result;
            TryGetValue(key, out result);
            return (float)(result ?? defaultValue);
        }

        public string GetAsSingle(string key, string defaultValue = "")
        {
            object result;
            TryGetValue(key, out result);
            return (string)(result ?? defaultValue);
        }

        public T GetAsEnum<T>(string key, T defaultValue = default(T))
            where T : struct
        {
            object result;
            TryGetValue(key, out result);
            return result != null && Enum.IsDefined(typeof(T), result)
                ? (T)result
                : defaultValue;
        }

        public Color GetAsColor(string key, Color defaultValue = default(Color))
        {
            object result;
            TryGetValue(key, out result);
            return (Color)(result ?? defaultValue);
        }

        public Color GetAsColor(
            string keyR,
            string keyG,
            string keyB,
            Color defaultValue = default(Color))
        {
            return new Color(
                GetAsInt32(keyR, defaultValue.R),
                GetAsInt32(keyR, defaultValue.G),
                GetAsInt32(keyR, defaultValue.B),
                255);
        }

        public Color GetAsColor(
            string keyR, 
            string keyG, 
            string keyB,
            string keyA,
            Color defaultValue = default(Color))
        {
            return new Color(
                GetAsInt32(keyR, defaultValue.R),
                GetAsInt32(keyR, defaultValue.G),
                GetAsInt32(keyR, defaultValue.B),
                GetAsInt32(keyR, defaultValue.A));
        }

        public Point GetAsPoint(
            string keyX, 
            string keyY, 
            Point defaultValue = default(Point))
        {
            return new Point(
                GetAsInt32(keyX, defaultValue.X),
                GetAsInt32(keyY, defaultValue.Y));
        }

        public Vector2 GetAsVector2(
            string keyX, 
            string keyY, 
            Vector2 defaultValue = default(Vector2))
        {
            return new Vector2(
                GetAsSingle(keyX, defaultValue.X),
                GetAsSingle(keyY, defaultValue.Y));
        }

        public Vector3 GetAsVector3(
            string keyX, 
            string keyY, 
            string keyZ,
            Vector3 defaultValue = default(Vector3))
        {
            return new Vector3(
                GetAsSingle(keyX, defaultValue.X),
                GetAsSingle(keyY, defaultValue.Y),
                GetAsSingle(keyZ, defaultValue.Z));
        }

        public Vector4 GetAsVector4(
            string keyX,
            string keyY,
            string keyZ,
            string keyW,
            Vector4 defaultValue = default(Vector4))
        {
            return new Vector4(
                GetAsSingle(keyX, defaultValue.X),
                GetAsSingle(keyY, defaultValue.Y),
                GetAsSingle(keyZ, defaultValue.Z),
                GetAsSingle(keyW, defaultValue.W));
        }

        public Rectangle GetAsRectangle(
            string keyX,
            string keyY,
            string keyW,
            string keyH,
            Rectangle defaultValue = default(Rectangle))
        {
            return new Rectangle(
                GetAsPoint(keyX, keyY, defaultValue.Location),
                GetAsPoint(keyW, keyH, defaultValue.Size));
        }
    }
}
