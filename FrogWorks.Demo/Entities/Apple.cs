using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class Apple : Entity
    {
        Image _image;

        public Apple(float x, float y)
            : this(new Vector2(x, y))
        {
        }

        public Apple(Vector2 position)
            : base()
        {
            var texture = Texture.Load(@"Textures\Apple");
            _image = new Image(texture, false);
            _image.CenterOrigin();
            Add(_image);

            Position = position;
            //Collider = new RectangleCollider(48f, 48f, -24f, -20f);
        }

        public bool IsOverlapping(Vector2 position)
        {
            return false; //return Collider?.Collide(position) ?? false;
        }
    }
}
