using System.IO;
using System.Xml;

namespace FrogWorks
{
    public sealed class TexturePacker
    {
        public static TextureAtlas Load(string filePath)
        {
            return AssetManager.GetFromCache(filePath, FromStream);
        }

        private static TextureAtlas FromStream(string filePath)
        {
            var stream = AssetManager.GetStream(filePath, ".xml");

            if (stream != null)
            {
                using (stream)
                {
                    var atlas = new TextureAtlas();
                    var document = new XmlDocument();
                    document.Load(stream);

                    var root = document["TextureAtlas"];
                    var directory = Path.GetDirectoryName(filePath);
                    var rootTexturePath = Path.Combine(directory, root.AttrToString("imagePath"));
                    var rootTexture = Texture.Load(rootTexturePath);

                    foreach (XmlElement spriteNode in root.ChildNodes)
                    {
                        var key = Path.ChangeExtension(spriteNode.AttrToString("n"), null);
                        var texture = rootTexture.ClipRegion(spriteNode.AttrToRectangle("x", "y", "w", "h"));
                        var size = spriteNode.AttrToVector2("oW", "oH");
                        var origin = spriteNode.AttrToVector2("oX", "oY");
                        var isRotated = spriteNode.HasAttribute("r");

                        atlas.Add(key, new TextureAtlasTexture(texture, size, origin, isRotated));
                    }

                    return atlas;
                }
            }

            return null;
        }
    }
}
