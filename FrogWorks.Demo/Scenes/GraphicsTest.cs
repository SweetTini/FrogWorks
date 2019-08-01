using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class GraphicsTest : Scene
    {
        public GraphicsTest()
            : base()
        {
            AddLayer("Sprites");

            var textEntity = new Text()
            {
                Y = 8,
                TextToDisplay = "Click Apple to wiggle it!\nArrow Keys - Scroll Camera\nWASD - Rotate/Scale Camera",
                HorizontalAlignment = HorizontalAlignment.Center,
                Depth = 100
            };

            var backgroundEntity = new Background()
            {
                Color = Color.LightSalmon,
                Depth = -100
            };

            AddEntitiesToLayer("Sprites", textEntity);
            AddEntities(backgroundEntity);

            CreateApple(Engine.Display.Width / 2f, Engine.Display.Height / 2f);
            CreateApple(Engine.Display.Width / 2f - 80f, Engine.Display.Height / 2f);
            CreateApple(Engine.Display.Width / 2f + 80f, Engine.Display.Height / 2f);
        }

        public override void Update(float deltaTime)
        {
            if (Input.Keyboard.IsPressed(Keys.Escape))
            {
                SetNextScene<TestMenu>();
                return;
            }

            var camera = MainLayer.Camera;

            camera.X += Input.Keyboard.GetAxis(Keys.RightArrow, Keys.LeftArrow) * 2f;
            camera.Y += Input.Keyboard.GetAxis(Keys.DownArrow, Keys.UpArrow) * 2f;
            camera.Zoom += Input.Keyboard.GetAxis(Keys.W, Keys.S) * .005f;
            camera.AngleInDegrees += Input.Keyboard.GetAxis(Keys.D, Keys.A) * .5f;

            var cursor = GetLayer("Sprites").Camera.ViewToWorld(Input.Mouse.Position);

            foreach (var apple in GetEntitiesOfType<Apple>())
            {
                if (apple.Contains(cursor) && Input.Mouse.IsClicked(MouseButton.Left))
                {
                    apple.Wiggle();
                }
            }

            base.Update(deltaTime);
        }

        private Apple CreateApple(float x, float y, int depth = 0)
        {
            var apple = new Apple()
            {
                Position = new Vector2(x, y),
                Depth = depth
            };

            AddEntitiesToLayer("Sprites", apple);
            return apple;
        }
    }
}
