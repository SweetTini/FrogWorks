using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrogWorks.Demo
{
    public class Apple : Entity
    {
        public Apple()
            : base()
        {
            var texture = Texture.Load(@"Textures\Apple");
            var image = new Image(texture, false);            
            image.CenterOrigin();
            Add(image);
        }
    }
}
