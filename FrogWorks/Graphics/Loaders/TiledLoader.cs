using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace FrogWorks
{
    public sealed class TiledLoader
    {
        const uint FlipHorizontally = 0x80000000,
                   FlipVertically = 0x40000000,
                   FlipDiagonally = 0x20000000;

        public static TileMapContainer Load(string filePath, string rootDirectory = "")
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
                rootDirectory = Runner.Application.ContentDirectory;

            using (var stream = File.OpenRead(Path.Combine(rootDirectory, filePath)))
            {
                var container = new TileMapContainer();
                Read(stream, container, Path.GetDirectoryName(filePath));
                return container;
            }
        }

        static void Read(FileStream stream, TileMapContainer container, string directory)
        {
            var document = new XmlDocument();
            document.Load(stream);

            var xmlRoot = document["map"];
            var infos = new List<TileSetInfo>();

            ReadMap(xmlRoot, container);
            ReadTileSets(xmlRoot, container, infos, directory);
            ReadLayers(xmlRoot, container, infos);
        }

        static void ReadMap(XmlElement xmlRoot, TileMapContainer container)
        {
            container.Size = new Point(xmlRoot.AttrToInt32("width"), 
                                       xmlRoot.AttrToInt32("height"));
            container.TileSize = new Point(xmlRoot.AttrToInt32("tilewidth"),
                                           xmlRoot.AttrToInt32("tileheight"));
        }

        static void ReadTileSets(XmlElement xmlRoot, 
                                 TileMapContainer container,
                                 List<TileSetInfo> infos, 
                                 string directory)
        {
            foreach (XmlElement xmlTileSet in xmlRoot.GetElementsByTagName("tileset"))
            {
                var properties = ReadProperties(xmlTileSet);

                var info = new TileSetInfo()
                {
                    Offset = xmlTileSet.AttrToInt32("firstgid"),
                    TileCount = xmlTileSet.AttrToInt32("tilecount"),
                    Source = Path.Combine(directory, xmlTileSet["image"].Attribute("source"))
                };

                if (properties.ContainsKey("referenceonly"))
                    info.ReferenceOnly = Convert.ToBoolean(properties["referenceonly"]);

                if (!info.ReferenceOnly)
                {
                    var texture = Texture.Load(info.Source);
                    info.TileSet = new TileSet(texture, container.TileWidth, container.TileHeight);
                }

                infos.Add(info);
            }
        }

        static void ReadLayers(XmlElement xmlRoot, 
                               TileMapContainer container, 
                               List<TileSetInfo> infos)
        {
            foreach (XmlElement xmlLayer in xmlRoot.GetElementsByTagName("layer"))
            {
                var layerName = xmlLayer.Attribute("name").ToLower();
                var tileData = ReadLayerData(xmlLayer["data"], container);
                CreateLayer(container, infos, tileData, layerName);
            }
        }

        static int[,] ReadLayerData(XmlElement xmlData, TileMapContainer container)
        {
            var tileData = new int[container.Columns, container.Rows];

            if (!string.IsNullOrEmpty(xmlData.Attribute("encoding")))
            {
                ReadEncodedLayerData(xmlData, tileData);
            }
            else
            {
                var index = 0;

                foreach (XmlElement xmlTile in xmlData.GetElementsByTagName("tile"))
                {
                    var gid = xmlTile.AttrToInt32("gid");
                    var x = index % tileData.GetLength(0);
                    var y = index / tileData.GetLength(0);

                    tileData[x, y] = gid;
                    index++;
                }
            }

            return tileData;
        }

        static void ReadEncodedLayerData(XmlElement xmlData, int[,] tileData)
        {
            if (xmlData.Attribute("encoding") != "base64")
                throw new Exception("Tiled supports Base64 encoding only.");

            var rawData = Convert.FromBase64String(xmlData.InnerText);
            var stream = new MemoryStream(rawData, false) as Stream;

            switch (xmlData.Attribute("compression"))
            {
                case "gzip":
                    stream = new GZipStream(stream, CompressionMode.Decompress);
                    break;
                case "zlib":
                    {
                        var size = rawData.Length - 6;
                        var data = new byte[size];

                        Array.Copy(rawData, 2, data, 0, size);
                        stream = new MemoryStream(data, false);
                        stream = new DeflateStream(stream, CompressionMode.Decompress);
                    }
                    break;
                default:
                    break;
            }

            using (stream)
            {
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < tileData.GetLength(0) * tileData.GetLength(1); i++)
                    {
                        var ugid = reader.ReadUInt32();
                        ugid &= ~(FlipHorizontally | FlipVertically | FlipDiagonally);

                        var x = i % tileData.GetLength(0);
                        var y = i / tileData.GetLength(0);
                        tileData[x, y] = (int)ugid;
                    }
                }
            }
        }

        static Dictionary<string, string> ReadProperties(XmlElement xmlElement)
        {
            var properties = new Dictionary<string, string>();
            var xmlProperties = xmlElement["properties"]?.GetElementsByTagName("property");

            if (xmlProperties != null)
                foreach (XmlElement xmlProperty in xmlProperties)
                    properties.Add(xmlProperty.Attribute("name").ToLower(), 
                                   xmlProperty.Attribute("value").ToLower());

            return properties;
        }

        static void CreateLayer(TileMapContainer container,
                                List<TileSetInfo> infos,
                                int[,] tileData,
                                string layerName)
        {
            var tileLayer = new TileMap(container.Columns, container.Rows,
                                      container.TileWidth, container.TileHeight);

            var tileLayerFilled = false;

            foreach (var info in infos)
            {
                var dataLayer = new int[container.Columns, container.Rows];
                var dataLayerFilled = false;

                for (int i = 0; i < tileData.GetLength(0) * tileData.GetLength(1); i++)
                {
                    var x = i % tileData.GetLength(0);
                    var y = i / tileData.GetLength(0);
                    var index = tileData[x, y];

                    if (index.Between(info.Offset, info.Offset + info.TileCount - 1))
                    {
                        index -= info.Offset;

                        if (info.ReferenceOnly)
                        {
                            dataLayer[x, y] = index + 1;
                            dataLayerFilled = true;
                        }
                        else
                        {
                            tileLayer.Fill(info.TileSet[index], x, y, 1, 1);
                            tileLayerFilled = true;
                        }
                    }
                }

                if (dataLayerFilled)
                {
                    var source = Path.GetFileNameWithoutExtension(info.Source)
                        .Replace(" ", string.Empty)
                        .Replace("-", string.Empty)
                        .Replace("_", string.Empty)
                        .ToLower();

                    container.DataLayers.Add($"{layerName}-{source}", dataLayer);
                }
            }

            if (tileLayerFilled) container.TileLayers.Add(layerName, tileLayer);
        }

        class TileSetInfo
        {
            public int Offset { get; set; }

            public int TileCount { get; set; }

            public string Source { get; set; }

            public bool ReferenceOnly { get; set; }

            public TileSet TileSet { get; set; }
        }
    }
}
