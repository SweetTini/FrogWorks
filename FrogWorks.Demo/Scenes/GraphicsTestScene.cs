using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class GraphicsTestScene : Scene
    {
        public GraphicsTestScene()
            : base()
        {
            AddLayer("Sprites");

            var textEntity = new TextEntity()
            {
                Y = 8,
                Text = "Click Apple to shake it!\nArrow Keys - Scroll Camera\nWASD - Rotate/Scale Camera",
                HorizontalAlignment = HorizontalAlignment.Center,
                Depth = 100
            };

            var backgroundEntity = new BackgroundEntity()
            {
                Color = Color.LightSalmon,
                Depth = -100
            };

            AddEntitiesToLayer("Sprites", textEntity);
            AddEntities(backgroundEntity);

            CreateApple(Engine.FrameWidth / 2f, Engine.FrameHeight / 2f);
            CreateApple(Engine.FrameWidth / 2f - 80f, Engine.FrameHeight / 2f);
            CreateApple(Engine.FrameWidth / 2f + 80f, Engine.FrameHeight / 2f);
        }

        public override void Update(float deltaTime)
        {
            var camera = MainLayer.Camera;

            camera.X += Input.Keyboard.GetAxis(Keys.RightArrow, Keys.LeftArrow) * 2f;
            camera.Y += Input.Keyboard.GetAxis(Keys.DownArrow, Keys.UpArrow) * 2f;
            camera.Zoom += Input.Keyboard.GetAxis(Keys.W, Keys.S) * .005f;
            camera.AngleInDegrees += Input.Keyboard.GetAxis(Keys.D, Keys.A) * .5f;

            base.Update(deltaTime);
        }

        private AppleEntity CreateApple(float x, float y, int depth = 0)
        {
            var apple = new AppleEntity()
            {
                Position = new Vector2(x, y),
                Depth = depth
            };

            AddEntitiesToLayer("Sprites", apple);
            return apple;
        }
    }
}
