using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class DisplayTest : Scene
    {
        public DisplayTest()
            : base()
        {
            var textEntity = new Text()
            {
                Y = Engine.Display.Height / 2f,
                TextToDisplay = "Press 1-6 to change scaling.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var backgroundEntity = new Background()
            {
                Color = Color.DarkCyan,
                Coefficient = Vector2.One * .5f,
                AutoScroll = true,
                Depth = -100
            };

            AddEntities(textEntity, backgroundEntity);
        }

        public override void Update(float deltaTime)
        {
            if (Input.Keyboard.IsPressed(Keys.Escape))
            {
                SetNextScene<TestMenu>();
                return;
            }

            if (Input.Keyboard.IsPressed(Keys.Alpha1))
                Engine.Display.Scaling = Scaling.None;
            else if (Input.Keyboard.IsPressed(Keys.Alpha2))
                Engine.Display.Scaling = Scaling.Fit;
            else if (Input.Keyboard.IsPressed(Keys.Alpha3))
                Engine.Display.Scaling = Scaling.PixelPerfect;
            else if (Input.Keyboard.IsPressed(Keys.Alpha4))
                Engine.Display.Scaling = Scaling.Stretch;
            else if (Input.Keyboard.IsPressed(Keys.Alpha5))
                Engine.Display.Scaling = Scaling.Extend;
            else if (Input.Keyboard.IsPressed(Keys.Alpha6))
                Engine.Display.Scaling = Scaling.Crop;

            base.Update(deltaTime);
        }
    }
}
