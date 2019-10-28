using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace FrogWorks
{
    public static class ShaderEffect
    {
        public static Effect Load(string filePath)
        {
            var absolutePath = Path.Combine(Runner.Application.ContentDirectory, filePath);
            var fileBytes = File.ReadAllBytes(absolutePath);
            return new Effect(Runner.Application.Game.GraphicsDevice, fileBytes);
        }
    }
}
