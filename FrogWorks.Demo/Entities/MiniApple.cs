using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class MiniApple : Entity
    {
        Image _image;

        public MiniApple(float x, float y)
            : this(new Vector2(x, y))
        {
        }

        public MiniApple(Vector2 position)
            : base()
        {
            var texture = Texture.Load(@"Textures\MiniApple");
            _image = new Image(texture, false);
            _image.CenterOrigin();
            Add(_image);

            Position = position;
            Collider = new CircleCollider(-13, -10, 12);
        }

        public void MarkAsMain()
        {
            _image.Color = Color.Purple;
        }
    }
}
