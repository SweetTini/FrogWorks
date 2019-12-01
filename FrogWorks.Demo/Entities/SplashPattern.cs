using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class SplashPattern : Entity
    {
        BackPattern _pattern;

        public SplashPattern()
            : base()
        {
            var texture = Texture.Load("Textures\\Splash.png");
            _pattern = new BackPattern(texture);
            _pattern.Color = Color.Red;
            Add(_pattern);
        }
    }
}
