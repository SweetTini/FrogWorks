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
            var tileSets = new Dictionary<int, TileSet>();

            ReadMap(xmlRoot, container);
            ReadTileSets(xmlRoot, container, tileSets, directory);
            ReadLayers(xmlRoot, container, tileSets);
        }

        static void ReadMap(XmlElement xmlRoot, TileMapContainer container)
        {
            container.Columns = xmlRoot.AttrToInt32("width");
            container.Rows = xmlRoot.AttrToInt32("height");
            container.TileWidth = xmlRoot.AttrToInt32("tilewidth");
            container.TileHeight = xmlRoot.AttrToInt32("tileheight");
        }

        static void ReadTileSets(XmlElement xmlRoot, 
                                 TileMapContainer container, 
                                 Dictionary<int, TileSet> tileSets, 
                                 string directory)
        {
            var xmlTileSets = xmlRoot.GetElementsByTagName("tileset");

            foreach (XmlElement xmlTileSet in xmlTileSets)
            {
                var firstGid = xmlTileSet.AttrToInt32("firstgid"); 
                var xmlImage = xmlTileSet["image"];
                var imageSource = xmlImage.Attribute("source");
                var texture = Texture.Load(Path.Combine(directory, imageSource));
                var tileSet = new TileSet(texture, container.TileWidth, container.TileHeight);

                tileSets.Add(firstGid, tileSet);
            }
        }

        static void ReadLayers(XmlElement xmlRoot, 
                               TileMapContainer container, 
                               Dictionary<int, TileSet> tileSets)
        {
            var xmlLayers = xmlRoot.GetElementsByTagName("layer");

            foreach (XmlElement xmlLayer in xmlLayers)
            {
                var name = xmlLayer.Attribute("name");
                var xmlData = xmlLayer["data"];
                var tileData = ReadLayerData(xmlData, container);
                var tileMap = CreateTileMap(container, tileSets, tileData);

                container.AddLayer(name, tileMap);
            }
        }

        static int[,] ReadLayerData(XmlElement xmlData, TileMapContainer container)
        {
            var tileData = new int[container.Columns, container.Rows];
            var isEncoded = !string.IsNullOrEmpty(xmlData.Attribute("encoding"));

            if (isEncoded)
            {
                ReadEncodedLayerData(xmlData, tileData);
            }
            else
            {
                var xmlTiles = xmlData.GetElementsByTagName("tile");
                var index = 0;

                foreach (XmlElement xmlTile in xmlTiles)
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
            var isBase64Encoded = xmlData.Attribute("encoding") == "base64";
            if (!isBase64Encoded) throw new Exception("Tiled supports Base64 encoding only.");

            var rawData = Convert.FromBase64String(xmlData.InnerText);
            var stream = new MemoryStream(rawData, false) as Stream;
            var compression = xmlData.Attribute("compression");

            switch (compression)
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
                    var size = tileData.GetLength(0) * tileData.GetLength(1);

                    for (int i = 0; i < size; i++)
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

        static TileMap CreateTileMap(TileMapContainer container,
                                     Dictionary<int, TileSet> tileSets,
                                     int[,] tileData)
        {
            var tileMap = new TileMap(container.Columns, container.Rows, 
                                      container.TileWidth, container.TileHeight);
            var size = tileData.GetLength(0) * tileData.GetLength(1);

            foreach (var pair in tileSets)
            {
                for (int i = 0; i < size; i++)
                {
                    var x = i % tileData.GetLength(0);
                    var y = i / tileData.GetLength(0);
                    var index = tileData[x, y];

                    if (index.Between(pair.Key, pair.Key + pair.Value.Count - 1))
                    {
                        index -= pair.Key;
                        tileMap.Fill(pair.Value[index], x, y, 1, 1);
                    }
                }
            }

            return tileMap;
        }
    }
}
