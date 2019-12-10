using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class SplashPattern : Entity
    {
        BackPattern _pattern;

        public SplashPattern(Color? color = null)
            : base()
        {
            var texture = Texture.Load("Textures\\Splash.png");
            _pattern = new BackPattern(texture);
            _pattern.Color = color ?? Color.White;
            Add(_pattern);
        }
    }
}
