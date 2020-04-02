using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class Spider : Entity
    {
        Image _image;
        TileMapField _field;
        Vector2 _velocity;

        public Spider(TileMapField field, float x, float y)
            : base()
        {
            Position = new Vector2(x, y);
            Collider = new BoxCollider(1, 2, 20, 20);

            var texture = Texture.Load(@"Textures\Spider");
            _image = new Image(texture, false);
            _image.CenterOrigin();
            _image.Position = Vector2.One * 12f;
            Add(_image);

            _field = field;
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            HandleInputs();
            ResolveHorizontalMovement();
            ResolveVerticalMovement();
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            Collider.Draw(batch, Color.Cyan);
        }

        void HandleInputs()
        {
            _velocity.X = Input.Keyboard.GetAxis(Keys.LeftArrow, Keys.RightArrow);
            _velocity.Y = Input.Keyboard.GetAxis(Keys.UpArrow, Keys.DownArrow);
        }

        void ResolveHorizontalMovement()
        {
            X += _velocity.X;

            if (_field.Overlaps(this, _velocity, Vector2.UnitX, out var depth))
            {
                X += depth.X;
                _velocity.X = 0f;
            }
        }

        void ResolveVerticalMovement()
        {
            Y += _velocity.Y;

            if (_field.Overlaps(this, _velocity, Vector2.UnitY, out var depth))
            {
                Y += depth.Y;
                _velocity.Y = 0f;
            }
        }
    }
}
