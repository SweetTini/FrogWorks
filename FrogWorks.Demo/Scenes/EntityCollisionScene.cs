using Microsoft.Xna.Framework;

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
            ClearColor = ColorEX.FromRGB(96, 96, 96);

            _apple = new MiniApple(64, 64);
            _apple.MarkAsControllable();
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

        void SpawnApples()
        {
            for (int i = 0; i < 50; i++)
            {
                Add(new MiniApple(
                    RandomEX.Current.Next(16, 240),
                    RandomEX.Current.Next(16, 208)));
            }

            _apple.MoveToTop();
        }
    }
}
