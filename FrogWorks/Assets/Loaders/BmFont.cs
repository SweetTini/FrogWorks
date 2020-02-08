using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FrogWorks
{
    public sealed class BmFont
    {
        public static BitmapFont Load(BmFontFileType fileType, string filePath)
        {
            var absolutePath = Path.Combine(Runner.Application.ContentDirectory, filePath);

            try
            {
                using (var stream = File.OpenRead(absolutePath))
                {
                    var font = new BitmapFont();

                    switch (fileType)
                    {
                        default:
                        case BmFontFileType.Binary:
                            BinaryFile.Read(stream, font, Path.GetDirectoryName(filePath));
                            break;
                        case BmFontFileType.Xml:
                            XmlFile.Read(stream, font, Path.GetDirectoryName(filePath));
                            break;
                    }

                    return font;
                }
            }
            catch
            {
                return null;
            }
        }

        #region Binary
        private class BinaryFile
        {
            public static void Read(FileStream stream, BitmapFont font, string directory)
            {
                using (var reader = new BinaryReader(stream))
                {
                    var textures = new List<Texture>();
                    int versionNumber = ValidateFormat(reader);
                    while (ReadBlock(reader, font, textures, directory, versionNumber))
                        continue;
                }
            }

            static int ValidateFormat(BinaryReader reader)
            {
                var format = new string(reader.ReadChars(3));
                var version = reader.ReadByte();
                
                if (format != "BMF")
                    throw new Exception("Cannot read file. Invalid file format detected.");

                return version;
            }

            static bool ReadBlock(BinaryReader reader, BitmapFont font, List<Texture> textures, string directory, int versionNumber)
            {
                if (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var blockType = (int)reader.ReadByte();
                    var blockSize = reader.ReadInt32();

                    switch (blockType)
                    {
                        case 2: ReadCommon(reader, font, blockSize); break;
                        case 3: ReadPages(reader, font, textures, directory, blockSize); break;
                        case 4: ReadChars(reader, font, textures, blockSize); break;
                        case 5: ReadKerningPairs(reader, font, blockSize); break;
                        default: reader.ReadBytes(blockSize); break;
                    }

                    return true;
                }

                return false;
            }

            static void ReadCommon(BinaryReader reader, BitmapFont font, int blockSize)
            {
                font.LineHeight = reader.ReadInt16();
                reader.ReadBytes(blockSize - 2);
            }

            static void ReadPages(BinaryReader reader, BitmapFont font, List<Texture> textures, string directory, int blockSize)
            {
                var pageFile = reader.ReadNullTerminatedString();
                var textSize = pageFile.Length + 1;
                var count = blockSize / textSize - 1;
                var texture = Texture.Load(Path.Combine(directory, pageFile));
                textures.Add(texture);

                for (int i = 0; i < count; i++)
                {
                    pageFile = new string(reader.ReadChars(textSize)).TrimEnd();
                    texture = Texture.Load(Path.Combine(directory, pageFile));
                    textures.Add(texture);
                }
            }

            static void ReadChars(BinaryReader reader, BitmapFont font, List<Texture> textures, int blockSize)
            {
                var count = blockSize / 20;

                for (int i = 0; i < count; i++)
                {
                    var ascii = reader.ReadInt32();
                    var bounds = new Rectangle(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
                    var offset = new Point(reader.ReadInt16(), reader.ReadInt16());
                    var spacing = reader.ReadInt16();
                    var page = (int)reader.ReadByte();
                    var texture = textures[page].ClipRegion(bounds);

                    var character = new BitmapCharacter(texture, ascii, offset, spacing);
                    font.Characters.Add((char)ascii, character);
                    reader.ReadByte();
                }
            }

            static void ReadKerningPairs(BinaryReader reader, BitmapFont font, int blockSize)
            {
                var count = blockSize / 10;

                for (int i = 0; i < count; i++)
                {
                    var ascii = reader.ReadInt32();
                    var nextAscii = reader.ReadInt32();
                    var kerning = (int)reader.ReadInt16();

                    var character = font[ascii];
                    character?.AddOrUpdateKerning(nextAscii, kerning);
                }
            }
        }
        #endregion

        #region XML
        private class XmlFile
        {
            public static void Read(FileStream stream, BitmapFont font, string directory)
            {
                var textures = new List<Texture>();
                var document = new XmlDocument();
                document.Load(stream);

                var root = document["font"];

                ReadCommon(root, font);
                ReadPages(root, font, textures, directory);
                ReadChars(root, font, textures);
                ReadKerningPairs(root, font);
            }

            static void ReadCommon(XmlElement root, BitmapFont font)
            {
                var common = root["common"];
                font.LineHeight = common.AttrToInt32("lineHeight");
            }

            static void ReadPages(XmlElement root, BitmapFont font, List<Texture> textures, string directory)
            {
                var pages = root["pages"];

                foreach (XmlElement page in pages.ChildNodes)
                {
                    var pageFile = page.AttrToString("file");
                    var texture = Texture.Load(Path.Combine(directory, pageFile));
                    textures.Add(texture);
                }
            }

            static void ReadChars(XmlElement root, BitmapFont font, List<Texture> textures)
            {
                var chars = root["chars"];

                foreach (XmlElement charInfo in chars)
                {
                    var ascii = charInfo.AttrToInt32("id");
                    var bounds = charInfo.AttrToRectangle();
                    var offset = charInfo.AttrToPoint("xoffset", "yoffset");
                    var spacing = charInfo.AttrToInt32("xadvance");
                    var page = charInfo.AttrToInt32("page");
                    var texture = textures[page].ClipRegion(bounds);

                    var character = new BitmapCharacter(texture, ascii, offset, spacing);
                    font.Characters.Add(ascii, character);
                }
            }

            static void ReadKerningPairs(XmlElement root, BitmapFont font)
            {
                var kernings = root["kernings"];

                foreach (XmlElement kerningInfo in kernings.ChildNodes)
                {
                    var ascii = kerningInfo.AttrToInt32("first");
                    var nextAscii = kerningInfo.AttrToInt32("second");
                    var kerning = kerningInfo.AttrToInt32("amount");

                    var character = font[ascii];
                    character?.AddOrUpdateKerning(nextAscii, kerning);
                }
            }
        }
        #endregion
    }

    public enum BmFontFileType
    {
        Binary,
        Xml
    }
}
