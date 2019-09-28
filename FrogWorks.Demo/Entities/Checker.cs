using FrogWorks.Demo.Scenes;

namespace FrogWorks.Demo.Entities
{
    public class Checker : Entity
    {
        public static Texture Pattern { get; } = Texture.Load("Images/Checker.png");

        public Checker()
            : base()
        {
            var tiledBackground = new BackgroundImage(Pattern, false);
            Components.Add(tiledBackground);
        }
    }
}
