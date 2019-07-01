using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks
{
    public class TiledImage : Image
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public TiledImage(Texture texture, bool isEnabled)
            : this(texture, texture.Width, texture.Height, isEnabled)
        {
        }

        public TiledImage(Texture texture, int width, int height, bool isEnabled)
            : base(texture, isEnabled)
        {
            Width = width;
            Height = height;
        }

        public override void Draw(RendererBatch batch)
        {
        }
    }
}
