using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class Apple : Entity
    {
        public Apple(float x, float y)
            : this(new Vector2(x, y))
        {
        }

        public Apple(Vector2 position)
            : base()
        {
            var texture = Texture.Load(@"Textures\Apple");
            var image = new Image(texture, false);            
            image.CenterOrigin();
            Add(image);

            Position = position;
            Collider = new RectangleCollider(48f, 48f, -24f, -20f);
        }

        public bool IsOverlapping(Vector2 position)
        {
            return Collider?.Collide(position) ?? false;
        }
    }
}
