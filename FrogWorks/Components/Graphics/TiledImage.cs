using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Components.Graphics
{
    class TiledImage : Image
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public TiledImage(Texture texture, bool isEnabled)
            : base(texture, isEnabled)
        {
        }
    }
}
