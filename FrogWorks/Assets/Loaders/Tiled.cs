using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;

namespace FrogWorks
{
    public sealed class Tiled
    {
        public static TileMapCollection Load(string filePath)
        {
            TileMapCollection collection;
            TileMapCollection.TryGetFromCache(filePath, Read, out collection);
            return collection;
        }

        static TileMapCollection Read(string filePath)
        {
            try
            {
                var absolutePath = Path.Combine(Runner.Application.ContentDirectory, filePath);

                using (var stream = File.OpenRead(absolutePath))
                {
                    var collection = new TileMapCollection();
                    var xmlDoc = new XmlDocument();
                    var rootDirectory = Path.GetDirectoryName(filePath);

                    xmlDoc.Load(stream);
                    ReadMap(collection, xmlDoc, rootDirectory);

                    return collection;
                }
            }
            catch
            {
                return null;
            }
        }

        static void ReadMap(TileMapCollection collection, XmlDocument xmlDoc, string rootDirectory)
        {
            var xmlRoot = xmlDoc["map"];

            collection.Size = new Point(xmlRoot.AttrToInt32("width"), xmlRoot.AttrToInt32("height"));
            collection.TileSize = new Point(xmlRoot.AttrToInt32("tilewidth"), xmlRoot.AttrToInt32("tileheight"));
            collection.BackgroundColor = xmlRoot.AttrToColor("backgroundcolor");

            var tileSets = ReadTileSets(collection, xmlRoot, rootDirectory);
            var layers = ReadLayers(collection, xmlRoot, tileSets);
            var objects = ReadObjectGroups(xmlRoot);

            layers.ForEach(x => BuildTileMap(collection, tileSets, x));
            objects.ForEach(x => BuildObject(collection, x));
        }

        static List<TiledTileSet> ReadTileSets(TileMapCollection collection, XmlElement xmlRoot, string rootDirectory)
        {
            var tileSets = new List<TiledTileSet>();

            foreach (XmlElement xmlTileSet in xmlRoot.GetElementsByTagName("tileset"))
            {
                var source = Path.Combine(rootDirectory, xmlTileSet["image"].AttrToString("source"));
                var texture = Texture.Load(source);

                tileSets.Add(new TiledTileSet()
                {
                    TileSet = texture != null ? new TileSet(texture, collection.TileSize) : null,
                    GidOffset = xmlTileSet.AttrToInt32("firstgid"),
                    TileCount = xmlTileSet.AttrToInt32("tilecount")
                });
            }

            return tileSets;
        }

        static List<TiledLayer> ReadLayers(TileMapCollection collection, XmlElement xmlRoot, List<TiledTileSet> tileSets)
        {
            var layers = new List<TiledLayer>();

            foreach (XmlElement xmlLayer in xmlRoot.GetElementsByTagName("layer"))
            {
                layers.Add(new TiledLayer()
                {
                    Name = xmlLayer.AttrToString("name").ToLower(),
                    GidMap = ReadLayer(collection, xmlLayer["data"]),
                    Properties = ReadProperties(xmlLayer)
                });
            }

            return layers;
        }

        static int[] ReadLayer(TileMapCollection collection, XmlElement xmlRoot)
        {
            var gidMap = new int[collection.Columns * collection.Rows];
            var hasEncoding = !string.IsNullOrEmpty(xmlRoot.AttrToString("encoding"));
            var compression = xmlRoot.AttrToEnum<TiledCompression>("compression");

            if (hasEncoding) ReadTileMap(xmlRoot, gidMap, compression);
            else ReadTileMap(xmlRoot, gidMap);

            return gidMap;
        }

        static void ReadTileMap(XmlElement xmlRoot, int[] gidMap)
        {
            var index = 0;

            foreach (XmlElement xmlTile in xmlRoot.GetElementsByTagName("tile"))
            {
                var gid = xmlTile.AttrToInt32("gid");
                gidMap[index] = gid;
                index++;
            }
        }

        static void ReadTileMap(XmlElement xmlRoot, int[] gidMap, TiledCompression compression)
        {
            var encoding = xmlRoot.AttrToString("encoding");
            if (encoding == "base64")
            {
                using (var stream = GetDecodedStream(xmlRoot, compression))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        const uint horizontal = 0x80000000;
                        const uint vertical = 0x40000000;
                        const uint diagonal = 0x20000000;

                        for (int i = 0; i < gidMap.Length; i++)
                        {
                            var ugid = reader.ReadUInt32();
                            ugid &= ~(horizontal | vertical | diagonal);
                            gidMap[i] = (int)ugid;
                        }
                    }
                }
            }
        }

        static List<TiledObject> ReadObjectGroups(XmlElement xmlRoot)
        {
            var objects = new List<TiledObject>();

            foreach (XmlElement xmlObjectGroup in xmlRoot.GetElementsByTagName("objectgroup"))
            {
                foreach (XmlElement xmlObject in xmlObjectGroup.GetElementsByTagName("object"))
                {
                    objects.Add(new TiledObject()
                    {
                        Name = xmlObject.AttrToString("name").ToLower(),
                        Type = xmlObject.AttrToString("type").ToLower(),
                        Region = xmlObject.AttrToRectangle(),
                        Properties = ReadProperties(xmlObject)
                    });
                }
            }

            return objects;
        }

        static Dictionary<string, object> ReadProperties(XmlElement xmlRoot)
        {
            var properties = new Dictionary<string, object>();
            var xmlProperties = xmlRoot["properties"]?.GetElementsByTagName("property");

            if (xmlProperties != null)
            {
                foreach (XmlElement xmlProp in xmlProperties)
                {
                    var name = xmlProp.AttrToString("name").ToLower();
                    object value;

                    switch (xmlProp.AttrToString("type"))
                    {
                        case "bool": value = xmlProp.AttrToBoolean("value"); break;
                        case "int": value = xmlProp.AttrToInt32("value"); break;
                        case "float": value = xmlProp.AttrToSingle("value"); break;
                        case "color": value = xmlProp.AttrToColor("value"); break;
                        default: value = xmlProp.AttrToString("value"); break;
                    }

                    properties.Add(name, value);
                }
            }

            return properties;
        }

        #region Import Methods
        static void BuildTileMap(TileMapCollection collection, List<TiledTileSet> tileSets, TiledLayer layer)
        {
            var tileMap = null as TileMap;

            for (int i = 0; i < layer.GidMap.Length; i++)
            {
                var gid = layer.GidMap[i];
                if (gid == 0) continue;
                var tileSet = tileSets.First(x => x.WithinOffset(gid));
                gid -= tileSet.GidOffset;

                if (tileSet.TileSet == null)
                {
                    BuildUnusedTile(collection, layer, i, gid);
                }
                else
                {
                    if (tileMap == null) tileMap = new TileMap(collection.Size, collection.TileSize);
                    var position = new Point(i % collection.Columns, i / collection.Columns);
                    var texture = tileSet.TileSet[gid];
                    tileMap.Fill(texture, position, new Point(1, 1));
                }
            }

            if (tileMap != null)
            {
                var newTileMap = new TileMapCollectionTileMap()
                {
                    GroupName = layer.Name,
                    Component = tileMap,
                };

                layer.Properties.ToList()
                    .ForEach(x => newTileMap._properties.Add(x.Key, x.Value));

                collection._tileMaps.Add(newTileMap);
            }
        }

        static void BuildUnusedTile(TileMapCollection collection, TiledLayer layer, int index, int gid)
        {
            var newTile = new TileMapCollectionTile()
            {
                Gid = gid,
                GroupName = layer.Name,
                Position = new Point(
                    index % collection.Columns, 
                    index / collection.Columns)
            };

            collection._unusedTiles.Add(newTile);
        }

        static void BuildObject(TileMapCollection collection, TiledObject obj)
        {
            var newObj = new TileMapCollectionObject()
            {
                Name = obj.Name,
                Type = obj.Type,
                Region = obj.Region.SnapToGrid(collection.TileSize),
            };

            obj.Properties.ToList()
                .ForEach(x => newObj._properties.Add(x.Key, x.Value));

            collection._objects.Add(newObj);
        }
        #endregion

        #region Dependencies
        static Stream GetDecodedStream(XmlElement xmlRoot, TiledCompression compression)
        {
            var rawData = Convert.FromBase64String(xmlRoot.InnerText);

            switch (compression)
            {
                case TiledCompression.Gzip:
                    return new GZipStream(
                        new MemoryStream(rawData, false),
                        CompressionMode.Decompress);
                case TiledCompression.Zlib:
                    {
                        var data = new byte[rawData.Length - 6];
                        Array.Copy(rawData, 2, data, 0, data.Length);
                        return new DeflateStream(
                            new MemoryStream(data, false),
                            CompressionMode.Decompress);
                    }
                default:
                    throw new Exception("Unrecognizable compression type.");
            }
        }

        class TiledTileSet
        {
            public TileSet TileSet { get; set; }

            public int GidOffset { get; set; }

            public int TileCount { get; set; }

            public bool WithinOffset(int index) => index.Between(GidOffset, GidOffset + TileCount - 1);
        }

        class TiledLayer
        {
            public string Name { get; set; }

            public int[] GidMap { get; set; }

            public Dictionary<string, object> Properties { get; set; }
        }

        class TiledObject
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public Rectangle Region { get; set; }

            public Dictionary<string, object> Properties { get; set; }
        }

        enum TiledCompression
        {
            Gzip,
            Zlib
        }
        #endregion
    }
}
