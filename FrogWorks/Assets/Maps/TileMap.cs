using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrogWorks
{
    public sealed class TileMap
    {
        internal List<TileMapLayout> _layouts;
        internal List<TileMapTile> _tiles;
        internal List<TileMapObject> _objects;

        public Point Size { get; internal set; }

        public int Columns => Size.X;

        public int Rows => Size.Y;

        public Point TileSize { get; internal set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public Color BackgroundColor { get; internal set; }

        public ReadOnlyCollection<TileMapLayout> Layouts { get; private set; }

        public ReadOnlyCollection<TileMapTile> Tiles { get; private set; }

        public ReadOnlyCollection<TileMapObject> Objects { get; private set; }

        internal TileMap()
            : base()
        {
            _layouts = new List<TileMapLayout>();
            _tiles = new List<TileMapTile>();
            _objects = new List<TileMapObject>();
            Layouts = new ReadOnlyCollection<TileMapLayout>(_layouts);
            Tiles = new ReadOnlyCollection<TileMapTile>(_tiles);
            Objects = new ReadOnlyCollection<TileMapObject>(_objects);
        }
    }

    public class TileMapLayout
    {
        internal Dictionary<string, object> _properties;

        public string GroupName { get; internal set; }

        public TileMapRenderer Renderer { get; internal set; }

        public TileMapProperties Properties { get; private set; }

        internal TileMapLayout()
            : base()
        {
            _properties = new Dictionary<string, object>();
            Properties = new TileMapProperties(_properties);
        }
    }

    public class TileMapTile
    {
        public int Gid { get; internal set; }

        public string GroupName { get; internal set; }

        public Point Position { get; internal set; }

        public int X => Position.X;

        public int Y => Position.Y;

        internal TileMapTile()
            : base()
        {
        }
    }

    public class TileMapObject
    {
        internal Dictionary<string, object> _properties;

        public string Name { get; internal set; }

        public string Type { get; internal set; }

        public Rectangle Region { get; internal set; }

        public Point Position => Region.Location;

        public int X => Region.X;

        public int Y => Region.Y;

        public Point Size => Region.Size;

        public int Width => Region.Width;

        public int Height => Region.Height;

        public TileMapProperties Properties { get; private set; }

        internal TileMapObject()
            : base()
        {
            _properties = new Dictionary<string, object>();
            Properties = new TileMapProperties(_properties);
        }
    }

    public class TileMapProperties : ReadOnlyDictionary<string, object>
    {
        public TileMapProperties(IDictionary<string, object> dictionary) 
            : base(dictionary)
        {
        }

        public int GetAsInt32(string key, int defaultValue = 0)
        {
            object result;
            TryGetValue(key, out result);
            return result != null && result is int
                ? (int)result
                : defaultValue;
        }

        public float GetAsSingle(string key, float defaultValue = 0f)
        {
            object result;
            TryGetValue(key, out result);
            return result != null && result is float
                ? (float)result
                : defaultValue;
        }

        public string GetAsString(string key, string defaultValue = "")
        {
            object result;
            TryGetValue(key, out result);
            return result != null && result is string
                ? (string)result
                : defaultValue;
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
            return result != null && result is Color
                ? (Color)result
                : defaultValue;
        }

        public Color GetAsColor(
            string keyR,
            string keyG,
            string keyB,
            Color defaultValue = default(Color))
        {
            return new Color(
                GetAsInt32(keyR, defaultValue.R),
                GetAsInt32(keyG, defaultValue.G),
                GetAsInt32(keyB, defaultValue.B),
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
                GetAsInt32(keyG, defaultValue.G),
                GetAsInt32(keyB, defaultValue.B),
                GetAsInt32(keyA, defaultValue.A));
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
