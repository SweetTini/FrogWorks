using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class EntityCollisionScene : Scene
    {
        Apple _apple;

        public EntityCollisionScene()
            : base()
        {
        }

        protected override void Begin()
        {
            _apple = new Apple(64, 64);

            Add(_apple);
            Add(new Apple(120, 100));
            Add(new Apple(200, 150));
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            var velocity = Vector2.Zero;
            velocity.X = Input.Keyboard.GetAxis(Keys.RightArrow, Keys.LeftArrow);
            velocity.Y = Input.Keyboard.GetAxis(Keys.DownArrow, Keys.UpArrow);
            _apple.Position += velocity * 2f;
        }

        protected override void BeforeDraw(RendererBatch batch)
        {
            batch.Configure(camera: Camera);
            batch.Begin();
            DrawBroadphase(batch, Color.Teal, Color.Cyan);
            batch.End();
        }
    }
}
