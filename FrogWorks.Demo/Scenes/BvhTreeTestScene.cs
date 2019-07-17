using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;
using System.Linq;

namespace FrogWorks.Demo.Scenes
{
    public class BvhTreeTestScene : Scene
    {
        private const int MaxApples = 20;

        public BvhTreeTestScene()
            : base()
        {
            BackgroundColor = Color.Gray;
        }

        public override void Update(float deltaTime)
        {
            var cursor = MainLayer.Camera.ViewToWorld(Input.Mouse.Position);
            var apples = GetEntitiesOfType<MiniAppleEntity>().ToList();

            if (Input.Mouse.IsClicked(MouseButton.Left))
            {
                var appleClicked = apples.Where(x => x.Contains(cursor)).FirstOrDefault();

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

        private MiniAppleEntity CreateApple(float x, float y, int depth = 0)
        {
            var apple = new MiniAppleEntity()
            {
                Position = new Vector2(x, y),
                Depth = depth
            };

            AddEntities(apple);
            return apple;
        }
    }
}
