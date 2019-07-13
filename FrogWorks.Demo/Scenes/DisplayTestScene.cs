using FrogWorks.Demo.Entities;
using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Scenes
{
    public class DisplayTestScene : Scene
    {
        protected TextEntity TextEntity { get; private set; }

        protected BackgroundEntity BackgroundEntity { get; private set; }

        public DisplayTestScene()
            : base()
        {
            TextEntity = new TextEntity()
            {
                Y = Engine.FrameHeight / 2f,
                Text = "Press 1-6 to change scaling.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            BackgroundEntity = new BackgroundEntity()
            {
                Color = Color.Gray,
                Depth = -100
            };

            AddEntities(TextEntity, BackgroundEntity);
        }

        public override void Update(float deltaTime)
        {
            if (Input.Keyboard.IsPressed(Keys.Alpha1))
                SetDisplayScaling(Scaling.None);
            if (Input.Keyboard.IsPressed(Keys.Alpha2))
                SetDisplayScaling(Scaling.Fit);
            if (Input.Keyboard.IsPressed(Keys.Alpha3))
                SetDisplayScaling(Scaling.PixelPerfect);
            if (Input.Keyboard.IsPressed(Keys.Alpha4))
                SetDisplayScaling(Scaling.Stretch);
            if (Input.Keyboard.IsPressed(Keys.Alpha5))
                SetDisplayScaling(Scaling.Extend);
            if (Input.Keyboard.IsPressed(Keys.Alpha6))
                SetDisplayScaling(Scaling.Crop);

            base.Update(deltaTime);
        }
    }
}
