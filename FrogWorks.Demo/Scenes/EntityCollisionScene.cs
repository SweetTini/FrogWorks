using Microsoft.Xna.Framework;
using System.Linq;

namespace FrogWorks.Demo
{
    public class EntityCollisionScene : Scene
    {
        MiniApple _apple;

        public EntityCollisionScene()
            : base()
        {
        }

        protected override void Begin()
        {
            _apple = new MiniApple(64, 64);
            _apple.MarkAsMain();
            Add(_apple);
            SpawnApples();
        }

        protected override void BeforeUpdate(float deltaTime)
        {
            var velocity = Vector2.Zero;
            velocity.X = Input.Keyboard.GetAxis(Keys.RightArrow, Keys.LeftArrow);
            velocity.Y = Input.Keyboard.GetAxis(Keys.DownArrow, Keys.UpArrow);
            _apple.Position += velocity * 2f;

            foreach (var apple in Overlaps(_apple).OfType<MiniApple>())
                apple.Destroy();

            if (this.CountType<MiniApple>() < 2)
                SpawnApples();
        }

        protected override void BeforeDraw(RendererBatch batch)
        {
            batch.Configure(camera: Camera);
            batch.Begin();
            DrawBroadphase(batch, Color.Teal * .5f, Color.Cyan * .5f);
            batch.End();
        }

        void SpawnApples()
        {
            for (int i = 0; i < 20; i++)
            {
                Add(new MiniApple(
                    RandomEX.Current.Next(16, 240),
                    RandomEX.Current.Next(16, 208)));
            }
        }
    }
}
