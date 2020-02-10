using System.IO;
using System.Xml;

namespace FrogWorks
{
    public sealed class TexturePacker
    {
        public static TextureAtlas Load(string filePath)
        {
            TextureAtlas atlas;
            TextureAtlas.TryGetFromCache(filePath, Read, out atlas);
            return atlas;
        }

        private static TextureAtlas Read(string filePath)
        {
            var absolutePath = Path.Combine(Runner.Application.ContentDirectory, filePath);

            try
            {
                using (var stream = File.OpenRead(absolutePath))
                {
                    var atlas = new TextureAtlas();
                    Read(stream, atlas, Path.GetDirectoryName(filePath));
                    return atlas;
                }
            }
            catch
            {
                return null;
            }
        }

        private static void Read(FileStream stream, TextureAtlas atlas, string directory)
        {
            var document = new XmlDocument();
            document.Load(stream);

            var root = document["TextureAtlas"];
            var imagePath = Path.Combine(directory, root.AttrToString("imagePath"));
            var rootTexture = Texture.Load(imagePath);

            foreach (XmlElement spriteNode in root.ChildNodes)
            {
                var key = Path.GetFileNameWithoutExtension(spriteNode.AttrToString("n"));
                var texture = rootTexture.ClipRegion(spriteNode.AttrToRectangle("x", "y", "w", "h"));
                var size = spriteNode.AttrToVector2("oW", "oH");
                var origin = spriteNode.AttrToVector2("oX", "oY");
                var isRotated = spriteNode.HasAttribute("r");

                atlas.Add(key, new TextureAltasTexture(texture, size, origin, isRotated));
            }
        }
    }
}
