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
                Y = Engine.FrameHeight / 2f,
                Text = "Press 1-6 to change scaling.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var backgroundEntity = new BackgroundEntity()
            {
                Color = Color.DarkCyan,
                Coefficient = Vector2.One * .5f,
                Autoscroll = true,
                Depth = -100
            };

            AddEntities(textEntity, backgroundEntity);
        }

        public override void Update(float deltaTime)
        {
            if (Input.Keyboard.IsPressed(Keys.Alpha1))
                SetDisplayScaling(Scaling.None);
            else if (Input.Keyboard.IsPressed(Keys.Alpha2))
                SetDisplayScaling(Scaling.Fit);
            else if (Input.Keyboard.IsPressed(Keys.Alpha3))
                SetDisplayScaling(Scaling.PixelPerfect);
            else if (Input.Keyboard.IsPressed(Keys.Alpha4))
                SetDisplayScaling(Scaling.Stretch);
            else if (Input.Keyboard.IsPressed(Keys.Alpha5))
                SetDisplayScaling(Scaling.Extend);
            else if (Input.Keyboard.IsPressed(Keys.Alpha6))
                SetDisplayScaling(Scaling.Crop);

            base.Update(deltaTime);
        }
    }
}
