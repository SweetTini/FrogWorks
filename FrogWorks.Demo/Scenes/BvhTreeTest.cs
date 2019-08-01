using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;
using System.Linq;

namespace FrogWorks.Demo.Scenes
{
    public class BvhTreeTest : Scene
    {
        private const int MaxApples = 20;

        public BvhTreeTest()
            : base()
        {
            BackgroundColor = Color.Gray;
        }

        public override void Update(float deltaTime)
        {
            if (Input.Keyboard.IsPressed(Keys.Escape))
            {
                SetNextScene<TestMenu>();
                return;
            }

            var cursor = MainLayer.Camera.ViewToWorld(Input.Mouse.Position);
            var apples = GetEntitiesOfType<MiniApple>().ToList();

            if (Input.Mouse.IsClicked(MouseButton.Left))
            {
                var appleClicked = apples.FirstOrDefault(apple => apple.Contains(cursor));

                if (appleClicked != null)
                {
                    appleClicked.Destroy();
                }
                else
                {
                    if (apples.Count < MaxApples)
                    {
                        CreateApple(cursor.X, cursor.Y);
                    }
                }
            }

            base.Update(deltaTime);
        }

        public override void Draw(RendererBatch batch)
        {
            DrawColliderTree(batch, Color.Red, Color.Purple);

            base.Draw(batch);
        }

        private MiniApple CreateApple(float x, float y, int depth = 0)
        {
            var apple = new MiniApple()
            {
                Position = new Vector2(x, y),
                Depth = depth
            };

            AddEntities(apple);
            return apple;
        }
    }
}
