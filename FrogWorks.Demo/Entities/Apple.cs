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
            Collider = new BoxCollider(-24, -20, 48, 48);
        }

        public bool Contains(Vector2 position)
        {
            return Collider.Contains(position);
        }
    }
}
