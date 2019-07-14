using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class DisplayTestScene : Scene
    {
        public DisplayTestScene()
            : base()
        {
            var textEntity = new TextEntity()
            {
                Y = Engine.Display.Height / 2f,
                Text = "Press 1-6 to change scaling.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var backgroundEntity = new BackgroundEntity()
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
